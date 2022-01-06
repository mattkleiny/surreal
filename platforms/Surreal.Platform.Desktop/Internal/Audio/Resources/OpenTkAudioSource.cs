using OpenTK.Audio.OpenAL;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;

namespace Surreal.Internal.Audio.Resources;

[DebuggerDisplay("Audio Source (Playing={IsPlaying}, Volume={Volume})")]
internal sealed class OpenTkAudioSource : AudioSource
{
  private readonly OpenTkAudioDevice device;
  private          float             volume;

  public OpenTkAudioSource(OpenTkAudioDevice device)
  {
    this.device = device;
  }

  public int Id { get; } = AL.GenSource();

  public override float Volume
  {
    get => volume;
    set => volume = value.Clamp(0f, 1f);
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
    var innerClip = (OpenTkAudioClip) clip;

    AL.Source(Id, ALSourcef.Gain, volume * device.MasterVolume);
    AL.Source(Id, ALSourcei.Buffer, innerClip.Id);
    AL.SourcePlay(Id);
  }

  protected override void Dispose(bool managed)
  {
    AL.DeleteSource(Id);

    base.Dispose(managed);
  }
}
