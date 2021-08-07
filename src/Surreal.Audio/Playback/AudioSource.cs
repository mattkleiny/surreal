using Surreal.Audio.Clips;

namespace Surreal.Audio.Playback
{
  public abstract class AudioSource : AudioResource
  {
    public abstract float Volume    { get; set; }
    public abstract bool  IsPlaying { get; }

    public abstract void Play(AudioClip clip);
  }
}