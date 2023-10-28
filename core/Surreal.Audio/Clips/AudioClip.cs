using Surreal.Assets;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// A clip of audio that can be played back via an audio device.
/// </summary>
[AssetType("bff38f41-a594-4a08-8f2e-8705ec134d89")]
public sealed class AudioClip(IAudioBackend backend) : Disposable
{
  /// <summary>
  /// The handle to the clip in the underlying audio backend.
  /// </summary>
  public AudioHandle Handle { get; } = backend.CreateAudioClip();

  /// <summary>
  /// The duration of the clip's audio
  /// </summary>
  public TimeSpan Duration { get; private set; } = TimeSpan.Zero;

  /// <summary>
  /// The sample rate of the clip's audio
  /// </summary>
  public AudioSampleRate Rate { get; private set; } = AudioSampleRate.Standard;

  /// <summary>
  /// The size of the clip in bytes.
  /// </summary>
  public Size Size { get; private set; } = Size.Zero;

  public void Write<T>(TimeSpan duration, AudioSampleRate rate, ReadOnlySpan<T> buffer)
    where T : unmanaged
  {
    Duration = duration;
    Rate = rate;

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
