using Surreal.Audio.Clips;

namespace Surreal.Audio.Playback
{
  /// <summary>Represents a source capable of playing <see cref="AudioClip"/>s.</summary>
  public abstract class AudioSource : AudioResource
  {
    public abstract float Volume    { get; set; }
    public abstract bool  IsPlaying { get; }

    public abstract void Play(AudioClip clip);
  }
}
