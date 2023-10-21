using Surreal.Audio.Clips;
using Surreal.Memory;
using Surreal.Resources;

namespace Surreal.Audio;

/// <summary>
/// A resource in the audio subsystem.
/// </summary>
public abstract class AudioAsset : TrackedAsset<AudioAsset>
{
  public static Size AllocatedBufferSize => GetSizeEstimate<AudioBuffer>();
  public static Size AllocatedClipSize => GetSizeEstimate<AudioClip>();
}
