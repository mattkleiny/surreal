using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;
using Surreal.Platform.Internal.Audio.Resources;

namespace Surreal.Platform.Internal.Audio
{
  internal sealed class HeadlessAudioDevice : IAudioDevice
  {
    private float masterVolume;

    public float MasterVolume
    {
      get => masterVolume;
      set => masterVolume = Maths.Clamp(value, 0f, 1f);
    }

    public void Play(AudioClip clip, float volume = 1)
    {
      // no-op
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
