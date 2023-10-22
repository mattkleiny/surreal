using Surreal.Audio;
using Surreal.Audio.Clips;

namespace Surreal.Scenes.Spatial;

/// <summary>
/// A <see cref="SceneNode2D"/> that plays audio.
/// </summary>
public class AudioPlayer2D(IAudioBackend backend) : SceneNode2D
{
  private readonly AudioSource _source = new(backend);

  /// <summary>
  /// True if this audio player should play when the scene starts.
  /// </summary>
  public bool PlayOnAwake { get; set; }

  /// <summary>
  /// True if the audio source is currently playing.
  /// </summary>
  public bool IsPlaying => _source.IsPlaying;

  /// <summary>
  /// True if the audio source should loop.
  /// </summary>
  public bool IsLooping
  {
    get => _source.IsLooping;
    set => _source.IsLooping = value;
  }

  /// <summary>
  /// The normalized volume of the audio source.
  /// </summary>
  public float Volume
  {
    get => _source.Volume;
    set => _source.Volume = value;
  }

  /// <summary>
  /// The <see cref="AudioClip"/> to play.
  /// </summary>
  public AudioClip? AudioClip { get; set; }

  /// <summary>
  /// Plays the given <see cref="AudioClip"/> on the audio source.
  /// </summary>
  public void Play()
  {
    if (AudioClip != null)
    {
      _source.Play(AudioClip);
    }
  }

  /// <summary>
  /// Stops the audio source.
  /// </summary>
  public void Stop()
  {
    _source.Stop();
  }

  protected override void OnAwake()
  {
    if (PlayOnAwake)
    {
      Play();
    }

    base.OnAwake();
  }

  protected override void OnDispose()
  {
    _source.Stop();
    _source.Dispose();

    base.OnDispose();
  }

  protected override void OnTransformUpdated()
  {
    _source.Position = new Vector3(GlobalPosition, 0f);

    base.OnTransformUpdated();
  }
}
