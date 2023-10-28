namespace Surreal.Audio.Clips;

/// <summary>
/// An audio source allows <see cref="AudioClip" />s to be played.
/// </summary>
public sealed class AudioSource(IAudioBackend backend) : Disposable
{
  private bool _isLooping;
  private Vector3 _position;
  private float _volume = 1f;
  private float _distanceFalloff = 1f;

  /// <summary>
  /// The handle of the audio source in the underlying audio backend.
  /// </summary>
  public AudioHandle Handle { get; } = backend.CreateAudioSource();

  /// <summary>
  /// True if the audio source is currently playing.
  /// </summary>
  public bool IsPlaying => backend.IsAudioSourcePlaying(Handle);

  /// <summary>
  /// The normalized volume of the audio source.
  /// </summary>
  public float Volume
  {
    get => _volume;
    set
    {
      _volume = value;
      backend.SetAudioSourceGain(Handle, value);
    }
  }

  /// <summary>
  /// The position of the audio source in 3D space.
  /// </summary>
  public Vector3 Position
  {
    get => _position;
    set
    {
      _position = value;
      backend.SetAudioSourcePosition(Handle, value);
    }
  }

  /// <summary>
  /// True if the audio source should loop.
  /// </summary>
  public bool IsLooping
  {
    get => _isLooping;
    set
    {
      _isLooping = value;
      backend.SetAudioSourceLooping(Handle, value);
    }
  }

  /// <summary>
  /// The radius at which the audio source can be heard.
  /// </summary>
  public float DistanceFalloff
  {
    get => _distanceFalloff;
    set
    {
      _distanceFalloff = value;
      backend.SetAudioSourceDistanceFalloff(Handle, value);
    }
  }

  /// <summary>
  /// Plays the given clip on this audio source.
  /// </summary>
  public void Play(AudioClip clip)
  {
    backend.PlayAudioSource(Handle, clip.Handle);
  }

  /// <summary>
  /// Stops playing the current clip.
  /// </summary>
  public void Stop()
  {
    backend.StopAudioSource(Handle);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteAudioSource(Handle);
    }

    base.Dispose(managed);
  }
}
