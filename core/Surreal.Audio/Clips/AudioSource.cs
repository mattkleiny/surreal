namespace Surreal.Audio.Clips;

/// <summary>
/// An audio source allows <see cref="AudioClip" />s to be played.
/// </summary>
public sealed class AudioSource(IAudioBackend backend) : AudioAsset
{
  private bool _isLooping;
  private float _volume;

  public AudioHandle Handle { get; } = backend.CreateAudioSource();

  public bool IsPlaying => backend.IsAudioSourcePlaying(Handle);

  public float Volume
  {
    get => _volume;
    set
    {
      _volume = value;
      backend.SetAudioSourceVolume(Handle, value);
    }
  }

  public bool IsLooping
  {
    get => _isLooping;
    set
    {
      _isLooping = value;
      backend.SetAudioSourceLooping(Handle, value);
    }
  }

  public void Play(AudioClip clip)
  {
    backend.PlayAudioSource(Handle, clip.Handle);
  }

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
