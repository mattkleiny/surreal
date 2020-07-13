using System;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Audio.SPI;

namespace Surreal.Audio {
  public sealed class AudioDevice : IAudioDevice, IDisposable {
    private readonly AudioSourcePool sourcePool;

    public AudioDevice(IAudioBackend backend, int initialVoices) {
      Backend    = backend;
      sourcePool = backend.Factory.CreateAudioSourcePool(initialVoices);
    }

    public IAudioBackend Backend { get; }
    public IAudioFactory Factory => Backend.Factory;

    public float MasterVolume {
      get => Backend.MasterVolume;
      set => Backend.MasterVolume = value;
    }

    public void Play(AudioClip clip, float volume = 1) {
      var audioSource = sourcePool.GetAudioSource();
      if (audioSource != null) {
        audioSource.Volume = volume;
        audioSource.Play(clip);
      }
    }

    public void Dispose() {
      sourcePool.Dispose();
    }
  }
}