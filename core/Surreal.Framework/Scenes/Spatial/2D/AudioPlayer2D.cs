using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Colors;
using Surreal.Graphics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Utilities;

namespace Surreal.Scenes.Spatial;

/// <summary>
/// A <see cref="SceneNode2D"/> that plays audio.
/// </summary>
public class AudioPlayer2D : SceneNode2D, IGizmoObject
{
  private AudioSource? _source;
  private bool _playOnReady = true;
  private bool _isLooping;
  private float _volume = 1f;
  private AudioClip? _audioClip;

  /// <summary>
  /// True if this audio player should play when the scene starts.
  /// </summary>
  public bool PlayOnReady
  {
    get => _playOnReady;
    set => SetField(ref _playOnReady, value);
  }

  /// <summary>
  /// True if the audio source is currently playing.
  /// </summary>
  public bool IsPlaying => _source?.IsPlaying ?? false;

  /// <summary>
  /// True if the audio source should loop.
  /// </summary>
  public bool IsLooping
  {
    get => _source?.IsLooping ?? false;
    set
    {
      if (SetField(ref _isLooping, value) && _source != null)
      {
        _source.IsLooping = value;
      }
    }
  }

  /// <summary>
  /// The normalized volume of the audio source.
  /// </summary>
  public float Volume
  {
    get => _source?.Volume ?? 0f;
    set
    {
      if (SetField(ref _volume, value) && _source != null)
      {
        _source.Volume = value;
      }
    }
  }

  /// <summary>
  /// The <see cref="AudioClip"/> to play.
  /// </summary>
  public AudioClip? AudioClip
  {
    get => _audioClip;
    set => SetField(ref _audioClip, value);
  }

  /// <summary>
  /// Plays the given <see cref="AudioClip"/> on the audio source.
  /// </summary>
  public void Play()
  {
    if (AudioClip != null)
    {
      _source?.Play(AudioClip);
    }
  }

  /// <summary>
  /// Stops the audio source.
  /// </summary>
  public void Stop()
  {
    _source?.Stop();
  }

  protected override void OnReady()
  {
    base.OnReady();

    if (PlayOnReady)
    {
      Play();
    }
  }

  protected override void OnEnterTree()
  {
    base.OnEnterTree();

    _source = new AudioSource(Services.GetServiceOrThrow<IAudioBackend>())
    {
      Volume = _volume,
      IsLooping = _isLooping,
      Position = new Vector3(GlobalPosition, 0f)
    };
  }

  protected override void OnExitTree()
  {
    base.OnExitTree();

    if (_source != null)
    {
      _source.Stop();
      _source.Dispose();

      _source = null;
    }
  }

  protected override void OnTransformUpdated()
  {
    base.OnTransformUpdated();

    if (_source != null)
    {
      _source.Position = new Vector3(GlobalPosition, 0f);
    }
  }

  void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
  {
    gizmos.DrawWireCircle(GlobalPosition, 4f, Color.Yellow);
  }
}
