using System.Diagnostics;
using OpenTK.Audio.OpenAL;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;

namespace Surreal.Internal.Audio.Resources;

[DebuggerDisplay("Audio Source (Playing={IsPlaying}, Volume={Volume})")]
internal sealed class OpenTKAudioSource : AudioSource, IHasNativeId
{
  private readonly int id = AL.GenSource();

  int IHasNativeId.Id => id;

  private readonly OpenTKAudioDevice device;
  private          float             volume;

  public OpenTKAudioSource(OpenTKAudioDevice device)
  {
    this.device = device;
  }

  public override float Volume
  {
    get => volume;
    set => volume = value.Clamp(0f, 1f);
  }

  public override bool IsPlaying
  {
    get
    {
      AL.GetSource(id, ALGetSourcei.SourceState, out var state);

      return state == (int)ALSourceState.Playing;
    }
  }

  public override void Play(AudioClip clip)
  {
    var innerClip = (OpenTKAudioClip)clip;

    AL.Source(id, ALSourcef.Gain, volume * device.MasterVolume);
    AL.Source(id, ALSourcei.Buffer, innerClip.Id);
    AL.SourcePlay(id);
  }

  protected override void Dispose(bool managed)
  {
    AL.DeleteSource(id);

    base.Dispose(managed);
  }
}
