using Surreal.Audio.Clips;
using Surreal.Audio.SPI;

namespace Surreal.Audio {
  public interface IAudioDevice {
    IAudioBackend Backend { get; }
    IAudioFactory Factory { get; }

    float MasterVolume { get; set; }

    void Play(AudioClip clip, float volume = 1f);
  }
}