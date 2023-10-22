using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Memory;

namespace Surreal.Audio;

/// <summary>
/// Base class for any audio assets
/// </summary>
public abstract class AudioAsset : TrackedAsset<AudioAsset>
{
  public static Size TotalBufferSize => GetSizeEstimate<AudioBuffer>();
  public static Size TotalClipSize => GetSizeEstimate<AudioClip>();
}
