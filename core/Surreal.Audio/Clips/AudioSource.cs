namespace Surreal.Audio.Clips;

/// <summary>An audio source allows <see cref="AudioClip"/>s to be played.</summary>
public sealed class AudioSource : AudioResource
{
  private readonly IAudioServer server;
  private float volume;
  private bool isLooping;

  public AudioSource(IAudioServer server)
  {
    this.server = server;

    Handle = server.CreateAudioSource();
  }

  public AudioHandle Handle { get; }

  public float Volume
  {
    get => volume;
    set
    {
      volume = value;
      server.SetAudioSourceVolume(Handle, value);
    }
  }

  public bool IsLooping
  {
    get => isLooping;
    set
    {
      isLooping = value;
      server.SetAudioSourceLooping(Handle, value);
    }
  }

  public void Play(AudioClip clip)
  {
    server.PlayAudioSource(Handle, clip.Handle);
  }

  public void Stop()
  {
    server.StopAudioSource(Handle);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteAudioSource(Handle);
    }

    base.Dispose(managed);
  }
}
