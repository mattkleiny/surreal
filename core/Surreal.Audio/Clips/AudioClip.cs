using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// A clip of audio that can be played back via an audio device.
/// </summary>
public sealed class AudioClip(IAudioBackend backend) : AudioAsset, IHasSizeEstimate
{
  public AudioHandle Handle { get; } = backend.CreateAudioClip();
  public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
  public AudioSampleRate Rate { get; private set; } = AudioSampleRate.Standard;
  public Size Size { get; private set; } = Size.Zero;

  public void Write<T>(TimeSpan duration, AudioSampleRate rate, ReadOnlySpan<T> buffer)
    where T : unmanaged
  {
    Duration = duration;
    Rate = rate;
    Size = buffer.CalculateSize();

    backend.WriteAudioClipData(Handle, rate, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteAudioClip(Handle);
    }

    base.Dispose(managed);
  }
}
