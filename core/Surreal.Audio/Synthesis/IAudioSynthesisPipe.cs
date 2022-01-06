namespace Surreal.Audio.Synthesis;

/// <summary>A pipe for <see cref="IAudioSynthesisPlan"/> output.</summary>
public interface IAudioSynthesisPipe
{
  IPipeReader CreateReader();
  IPipeWriter CreateWriter();

  /// <summary>The reading end of the pipe.</summary>
  public interface IPipeReader : IDisposable, IAsyncDisposable
  {
    ValueTask<Memory<byte>> ReadAsync(CancellationToken cancellationToken = default);
  }

  /// <summary>The writing end of the pipe.</summary>
  public interface IPipeWriter : IDisposable, IAsyncDisposable
  {
    Memory<byte> GetBuffer(int minimumSize);
    void         Advance(int bytesWritten);

    ValueTask FlushAsync();
  }
}
