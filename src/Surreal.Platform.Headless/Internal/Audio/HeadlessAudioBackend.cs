using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Audio.SPI;
using Surreal.Platform.Internal.Audio.Resources;

namespace Surreal.Platform.Internal.Audio
{
  internal sealed class HeadlessAudioBackend : IAudioBackend, IAudioFactory
  {
    private float masterVolume;

    public IAudioFactory Factory => this;

    public float MasterVolume
    {
      get => masterVolume;
      set => masterVolume = Maths.Clamp(value, 0f, 1f);
    }

    public AudioClip CreateAudioClip(IAudioData data)
    {
      return new HeadlessAudioClip();
    }

    public AudioSource CreateAudioSource()
    {
      return new HeadlessAudioSource();
    }
  }
}