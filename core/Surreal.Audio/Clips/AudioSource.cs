namespace Surreal.Audio.Clips;

/// <summary>
/// An audio source allows <see cref="AudioClip" />s to be played.
/// </summary>
public sealed class AudioSource(IAudioBackend backend) : AudioAsset
{
  private bool _isLooping;
  private float _volume;

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
      backend.SetAudioSourceVolume(Handle, value);
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
