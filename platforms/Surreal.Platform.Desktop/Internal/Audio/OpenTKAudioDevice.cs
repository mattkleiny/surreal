using System;
using OpenTK.Audio;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;
using Surreal.Platform.Internal.Audio.Resources;

namespace Surreal.Platform.Internal.Audio {
  internal sealed class OpenTKAudioDevice : IAudioDevice, IDisposable {
    private readonly AudioContext    context = new();
    private readonly AudioSourcePool sourcePool;

    public OpenTKAudioDevice() {
      sourcePool = (this as IAudioDevice).CreateAudioSourcePool(capacity: 32);
    }

    private float masterVolume;

    public float MasterVolume {
      get => masterVolume;
      set => masterVolume = Maths.Clamp(value, 0f, 1f);
    }

    public void Play(AudioClip clip, float volume = 1) {
      var audioSource = sourcePool.GetAudioSource();
      if (audioSource != null) {
        audioSource.Volume = volume;
        audioSource.Play(clip);
      }
    }

    public AudioClip CreateAudioClip(IAudioData data) {
      return new OpenTKAudioClip(data);
    }

    public AudioSource CreateAudioSource() {
      return new OpenTKAudioSource(this);
    }

    public void Dispose() {
      context.Dispose();
    }
  }
}