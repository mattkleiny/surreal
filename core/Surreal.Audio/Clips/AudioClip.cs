using Surreal.Assets;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// A clip of audio that can be played back via an audio device.
/// </summary>
public sealed class AudioClip : AudioResource, IHasSizeEstimate
{
  private readonly IAudioServer _server;

  public AudioClip(IAudioServer server)
  {
    _server = server;

    Handle = server.CreateAudioClip();
  }

  public AudioHandle Handle { get; }
  public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
  public AudioSampleRate Rate { get; private set; } = AudioSampleRate.Standard;
  public Size Size { get; private set; } = Size.Zero;

  public void Write<T>(TimeSpan duration, AudioSampleRate rate, ReadOnlySpan<T> buffer)
    where T : unmanaged
  {
    Duration = duration;
    Rate = rate;
    Size = buffer.CalculateSize();

    _server.WriteAudioClipData(Handle, rate, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _server.DeleteAudioClip(Handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader : AssetLoader<AudioClip>
{
  private readonly IAudioServer _server;

  public AudioClipLoader(IAudioServer server)
  {
    _server = server;
  }

  public override async Task<AudioClip> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(_server);

    clip.Write<byte>(buffer.Duration, buffer.Rate, buffer.Span);

    return clip;
  }
}
