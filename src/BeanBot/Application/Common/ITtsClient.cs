using Google.Protobuf;

namespace BeanBot.Application.Common
{
  public interface ITtsClient
  {
    public Task CreateAsync();

    public Task<ByteString> SynthesizeSpeechAsync(string message);
  }
}
