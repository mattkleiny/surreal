using Surreal.Audio.Clips;
using Surreal.Memory;
using Surreal.Objects;

namespace Surreal.Audio;

/// <summary>A resource in the audio subsystem.</summary>
public abstract class AudioResource : TrackedNativeResource<AudioResource>
{
  public static Size AllocatedClipSize => GetSizeEstimate<AudioClip>();
}
