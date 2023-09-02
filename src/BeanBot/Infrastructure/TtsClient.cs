using BeanBot.Application.Common;
using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf;

namespace BeanBot.Infrastructure
{
  internal class TtsClient : ITtsClient
  {
    private static TextToSpeechClient _google;

    public async Task CreateAsync(CancellationToken cancellationToken)
    {
      _google ??= await TextToSpeechClient.CreateAsync(cancellationToken);
    }

    public async Task<ByteString> SynthesizeSpeechAsync(string message, CancellationToken cancellationToken)
    {
      var response = await _google.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
      {
        Input = new SynthesisInput { Text = message },
        Voice = new VoiceSelectionParams { LanguageCode = "en-AU", Name = "en-AU-Neural2-A" },
        AudioConfig = new AudioConfig { AudioEncoding = AudioEncoding.Mp3 }
      }, cancellationToken);

      return response.AudioContent;
    }
  }
}
