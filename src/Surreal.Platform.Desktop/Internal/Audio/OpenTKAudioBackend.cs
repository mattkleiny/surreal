using System;
using OpenTK.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Audio.SPI;
using Surreal.Platform.Internal.Audio.Resources;
using MathF = Surreal.Mathematics.MathF;

namespace Surreal.Platform.Internal.Audio {
  internal sealed class OpenTKAudioBackend : IAudioBackend, IDisposable {
    private readonly AudioContext context = new AudioContext();

    private float masterVolume;

    public IAudioBackend Factory => this;

    public float MasterVolume {
      get => masterVolume;
      set => masterVolume = MathF.Clamp(value, 0f, 1f);
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