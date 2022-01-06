namespace Surreal.Audio.Synthesis;

/// <summary>A simple tool for synthesizing audio from graphs.</summary>
public sealed class AudioSynthesizer : IDisposable
{
  private readonly IAudioSynthesisPlan plan;
  private readonly AudioSynthesisPipe  pipe = new();

  private Task?                    task;
  private CancellationTokenSource? cancellationSource;

  public AudioSynthesizer(IAudioSynthesisPlan plan)
  {
    this.plan = plan;
  }

  public void Start()
  {
    if (task != null)
    {
      throw new InvalidOperationException("The synthesizer is already emitting, cannot start twice!");
    }

    cancellationSource = new CancellationTokenSource();

    task = Task.Run(
      () => plan.EmitAsync(pipe, cancellationSource.Token),
      cancellationSource.Token
    );
  }

  public void Stop()
  {
    cancellationSource?.Cancel();
    cancellationSource = null;
    task               = null;
  }

  public void Dispose()
  {
    Stop();
  }

  /// <summary>The default <see cref="IAudioSynthesisPipe"/> implementation for this synthesizer.</summary>
  private sealed class AudioSynthesisPipe : IAudioSynthesisPipe
  {
    private readonly Memory<byte> buffer = new byte[4096];

    public IAudioSynthesisPipe.IPipeReader CreateReader() => new PipeReader(this);
    public IAudioSynthesisPipe.IPipeWriter CreateWriter() => new PipeWriter(this);

    /// <summary>The default reader implementation.</summary>
    private sealed class PipeReader : IAudioSynthesisPipe.IPipeReader
    {
      private readonly AudioSynthesisPipe pipe;

      public PipeReader(AudioSynthesisPipe pipe)
      {
        this.pipe = pipe;
      }

      public ValueTask<Memory<byte>> ReadAsync(CancellationToken cancellationToken = default)
      {
        return new(pipe.buffer);
      }

      public void      Dispose()      => throw new NotImplementedException();
      public ValueTask DisposeAsync() => throw new NotImplementedException();
    }

    /// <summary>The default writer implementation.</summary>
    private sealed class PipeWriter : IAudioSynthesisPipe.IPipeWriter
    {
      private readonly AudioSynthesisPipe pipe;

      public PipeWriter(AudioSynthesisPipe pipe)
      {
        this.pipe = pipe;
      }

      public Memory<byte> GetBuffer(int minimumSize)
      {
        return pipe.buffer;
      }

      public void Advance(int bytesWritten)
      {
        // no-op
      }

      public ValueTask FlushAsync()
      {
        return ValueTask.CompletedTask; // no-op
      }

      public void      Dispose()      => throw new NotImplementedException();
      public ValueTask DisposeAsync() => throw new NotImplementedException();
    }
  }
}
