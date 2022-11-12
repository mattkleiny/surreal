using Surreal.Audio.Clips;
using Surreal.Memory;

namespace Surreal.Audio;

/// <summary>A resource in the audio subsystem.</summary>
public abstract class AudioResource : TrackedResource<AudioResource>
{
  public static Size AllocatedBufferSize => GetSizeEstimate<AudioBuffer>();
  public static Size AllocatedClipSize => GetSizeEstimate<AudioClip>();
}


