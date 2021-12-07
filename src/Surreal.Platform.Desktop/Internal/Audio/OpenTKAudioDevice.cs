using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;
using Surreal.Platform.Internal.Audio.Resources;

namespace Surreal.Platform.Internal.Audio;

internal sealed class OpenTKAudioDevice : IAudioDevice, IDisposable
{
  private float masterVolume;

  public float MasterVolume
  {
    get => masterVolume;
    set => masterVolume = Maths.Clamp(value, 0f, 1f);
  }

  public AudioClip CreateAudioClip(IAudioData data)
  {
    return new OpenTKAudioClip(data);
  }

  public AudioSource CreateAudioSource()
  {
    return new OpenTKAudioSource(this);
  }

  public void Dispose()
  {
  }
}
