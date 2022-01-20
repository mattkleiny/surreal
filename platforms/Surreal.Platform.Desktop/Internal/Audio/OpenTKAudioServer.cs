using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Internal.Audio.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Audio;

internal sealed class OpenTKAudioServer : IAudioServer
{
  private float masterVolume;

  public float MasterVolume
  {
    get => masterVolume;
    set => masterVolume = value.Clamp(0f, 1f);
  }

  public AudioClip CreateAudioClip(IAudioData data)
  {
    return new OpenTKAudioClip(data);
  }

  public AudioSource CreateAudioSource()
  {
    return new OpenTKAudioSource(this);
  }
}
