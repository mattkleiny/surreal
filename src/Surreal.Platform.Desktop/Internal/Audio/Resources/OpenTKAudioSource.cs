using System.Diagnostics;
using OpenTK.Audio.OpenAL;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Platform.Internal.Audio.Resources
{
  [DebuggerDisplay("Audio Source (Playing={IsPlaying}, Volume={Volume})")]
  internal sealed class OpenTKAudioSource : AudioSource
  {
    public readonly int Id = AL.GenSource();

    private readonly OpenTKAudioBackend backend;
    private          float              volume;

    public OpenTKAudioSource(OpenTKAudioBackend backend)
    {
      this.backend = backend;
    }

    public override float Volume
    {
      get => volume;
      set => volume = Maths.Clamp(value, 0f, 1f);
    }

    public override bool IsPlaying
    {
      get
      {
        AL.GetSource(Id, ALGetSourcei.SourceState, out var state);

        return state == (int) ALSourceState.Playing;
      }
    }

    public override void Play(AudioClip clip)
    {
      var innerClip = (OpenTKAudioClip) clip;

      AL.Source(Id, ALSourcef.Gain, volume * backend.MasterVolume);
      AL.Source(Id, ALSourcei.Buffer, innerClip.Id);
      AL.SourcePlay(Id);
    }

    protected override void Dispose(bool managed)
    {
      AL.DeleteSource(Id);

      base.Dispose(managed);
    }
  }
}
