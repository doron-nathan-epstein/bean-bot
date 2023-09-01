using BeanBot.Application.Audio;
using BeanBot.Application.Common;
using Discord;
using Discord.Audio;
using NAudio.Wave;
using System.Collections.Concurrent;
using System.Text;

namespace BeanBot
{
  internal sealed partial class AudioService : IAudioService
  {
    private static readonly ConcurrentDictionary<ulong, AudioClient> _channels = new();
    private static ITtsClient _ttsClient;

    public AudioService(ITtsClient ttsClient)
    {
      //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "tts_auth.json");
      _ttsClient = ttsClient;
    }

    public partial class AudioQueue
    {
      private static readonly ConcurrentQueue<AudioInQueue> _queue = new();
      private static bool _taskIsRunning = false;
      private static CancellationTokenSource _cancel;

      private class AudioInQueue
      {
        public ulong Server { get; set; }
        public string Message { get; set; }
        public AudioType Type { get; set; }
      }

      public static void Enqueue(ulong serverId, string message, AudioType type)
      {
        _queue.Enqueue(new AudioInQueue { Server = serverId, Message = message, Type = type });
        if (!_taskIsRunning)
          Task.Run(ProcessQueuedItemsAsync);
      }

      public static async void SkipQueue(ITextChannel channel)
      {
        if (_cancel != null)
          _cancel.Cancel();
        else
          await channel.SendMessageAsync("There is nothing in the queue to skip");
      }

      private static async Task ProcessQueuedItemsAsync()
      {
        _taskIsRunning = true;
        while (true)
        {
          if (!_queue.Any()) break;
          if (_queue.TryDequeue(out AudioInQueue item))
          {
            _cancel = new CancellationTokenSource();
            try
            {
              if (item.Type == AudioType.TTS)
                await ProcessTextToSpeech(item.Server, item.Message, _cancel.Token);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { _cancel.Dispose(); }
          }
        }
        _taskIsRunning = false;
      }
    }

    private class AudioClient
    {
      public ulong ChannelId { get; set; }
      public IAudioClient Client { get; set; }
      public AudioOutStream AudioDevice { get; set; }
      public AudioQueue Queue { get; set; }
    }

    private static string MD5(string input)
    {
      using var md5 = System.Security.Cryptography.MD5.Create();
      byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
      return Convert.ToHexString(hashBytes);
    }

    private static async Task ProcessTextToSpeech(ulong server, string message, CancellationToken cancellationToken)
    {
      if (!Directory.Exists("tts"))
        Directory.CreateDirectory("tts");

      string file_Name = Path.Combine("tts", string.Format("{0}.mp3", MD5(message.ToLower())));

      if (!File.Exists(file_Name))
      {
        await _ttsClient.CreateAsync(cancellationToken);

        var audioContent = await _ttsClient.SynthesizeSpeechAsync(message, cancellationToken);

        using var stream = File.Create(file_Name);
        audioContent.WriteTo(stream);
      }

      await SendMp3AudioAsync(server, file_Name, cancellationToken);
    }

    private static async Task SendMp3AudioAsync(ulong server, string audio_file, CancellationToken cancellationToken)
    {
      if (!File.Exists(audio_file)) return;

      if (_channels.TryGetValue(server, out AudioClient voice))
      {
        using var audio = new Mp3FileReader(audio_file);
        var stream = new WaveFormatConversionStream(new WaveFormat(48000, 16, 2), audio);

        voice.AudioDevice ??= voice.Client.CreatePCMStream(AudioApplication.Mixed, 98304, 200);

        try { await stream.CopyToAsync(voice.AudioDevice, 1920, cancellationToken); } finally { await voice.AudioDevice.FlushAsync(cancellationToken); }
      }
    }

    public async Task AddQueueAsync(IGuild server, string message, AudioType type)
    {
      if (_channels.TryGetValue(server.Id, out _))
        AudioQueue.Enqueue(server.Id, message, type);

      await Task.CompletedTask;
    }

    public async Task SkipAudioAsync(IGuild server, ITextChannel textChannel)
    {
      if (_channels.TryGetValue(server.Id, out _))
        AudioQueue.SkipQueue(textChannel);

      await Task.CompletedTask;
    }

    public async Task JoinAudioAsync(IGuild server, IVoiceChannel voiceChannel, ITextChannel textChannel)
    {
      if (voiceChannel.Guild.Id != server.Id) return;

      // Checkk if currently connected vc
      if (_channels.TryGetValue(server.Id, out AudioClient voice))
      {
        if (voiceChannel.Id == voice.ChannelId && voice.Client.ConnectionState == ConnectionState.Connected)
        {
          await textChannel.SendMessageAsync("Already connected to the same channel.");
          return;
        }

        var oldVoice = voice; // Make a temporary copy of the old audioclient
        var client = await voiceChannel.ConnectAsync(); // This should disconnect from any current channel and connect to newly desired
        voice.AudioDevice = null;
        voice.Client = client;
        voice.ChannelId = voiceChannel.Id;
        _channels.TryUpdate(server.Id, voice, oldVoice);
        return;
      }

      voice = new AudioClient
      {
        ChannelId = voiceChannel.Id,
        Queue = new AudioQueue()
      };

      var audioClient = await voiceChannel.ConnectAsync();
      voice.Client = audioClient;
      if (!_channels.TryAdd(server.Id, voice))
        await textChannel.SendMessageAsync("Failed to add to our hashmap, internal error");
    }

    public async Task LeaveAudioAsync(IGuild server)
    {
      if (_channels.TryRemove(server.Id, out AudioClient voice))
        voice.Client.Dispose();

      await Task.CompletedTask;
    }
  }
}
