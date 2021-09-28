using Surreal.Audio.Clips;
using Surreal.Memory;
using Surreal.Objects;

namespace Surreal.Audio
{
  /// <summary>Base class for any audio resource.</summary>
  public abstract class AudioResource : TrackedNativeResource<AudioResource>
  {
    public static Size AllocatedClipSize => GetSizeEstimate<AudioClip>();
  }
}
