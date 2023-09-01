using Google.Protobuf;

namespace BeanBot.Application.Common
{
  public interface ITtsClient
  {
    public Task CreateAsync(CancellationToken cancellationToken);

    public Task<ByteString> SynthesizeSpeechAsync(string message, CancellationToken cancellationToken);
  }
}
