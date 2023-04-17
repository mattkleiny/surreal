namespace Surreal.Audio.Clips;

/// <summary>
/// An audio source allows <see cref="AudioClip" />s to be played.
/// </summary>
public sealed class AudioSource : AudioResource
{
  private readonly IAudioServer _server;
  private bool _isLooping;
  private float _volume;

  public AudioSource(IAudioServer server)
  {
    _server = server;

    Handle = server.CreateAudioSource();
  }

  public AudioHandle Handle { get; }

  public bool IsPlaying => _server.IsAudioSourcePlaying(Handle);

  public float Volume
  {
    get => _volume;
    set
    {
      _volume = value;
      _server.SetAudioSourceVolume(Handle, value);
    }
  }

  public bool IsLooping
  {
    get => _isLooping;
    set
    {
      _isLooping = value;
      _server.SetAudioSourceLooping(Handle, value);
    }
  }

  public void Play(AudioClip clip)
  {
    _server.PlayAudioSource(Handle, clip.Handle);
  }

  public void Stop()
  {
    _server.StopAudioSource(Handle);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _server.DeleteAudioSource(Handle);
    }

    base.Dispose(managed);
  }
}
