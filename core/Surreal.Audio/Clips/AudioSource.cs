﻿namespace Surreal.Audio.Clips;

/// <summary>
/// An audio source allows <see cref="AudioClip" />s to be played.
/// </summary>
public sealed class AudioSource : AudioResource
{
  private readonly IAudioBackend _backend;
  private bool _isLooping;
  private float _volume;

  public AudioSource(IAudioBackend backend)
  {
    _backend = backend;

    Handle = backend.CreateAudioSource();
  }

  public AudioHandle Handle { get; }

  public bool IsPlaying => _backend.IsAudioSourcePlaying(Handle);

  public float Volume
  {
    get => _volume;
    set
    {
      _volume = value;
      _backend.SetAudioSourceVolume(Handle, value);
    }
  }

  public bool IsLooping
  {
    get => _isLooping;
    set
    {
      _isLooping = value;
      _backend.SetAudioSourceLooping(Handle, value);
    }
  }

  public void Play(AudioClip clip)
  {
    _backend.PlayAudioSource(Handle, clip.Handle);
  }

  public void Stop()
  {
    _backend.StopAudioSource(Handle);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _backend.DeleteAudioSource(Handle);
    }

    base.Dispose(managed);
  }
}
