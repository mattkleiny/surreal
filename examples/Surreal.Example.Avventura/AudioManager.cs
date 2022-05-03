using Surreal.Assets;
using Surreal.IO;

namespace Avventura;

public readonly record struct SoundBite(VirtualPath Path)
{
  public static SoundBite    ActorDamage1 { get; } = new("Assets/audio/actor-damage-1.wav");
  public static SoundBite    ActorDamage2 { get; } = new("Assets/audio/actor-damage-2.wav");
  public static SoundBite    ActorDamage3 { get; } = new("Assets/audio/actor-damage-3.wav");
  public static SoundBiteSet ActorDamage  { get; } = new(ActorDamage1, ActorDamage2, ActorDamage3);

  public static SoundBite    ActorFootstepSoft1 { get; } = new("Assets/audio/actor-footstep-soft-1.wav");
  public static SoundBite    ActorFootstepSoft2 { get; } = new("Assets/audio/actor-footstep-soft-2.wav");
  public static SoundBite    ActorFootstepSoft3 { get; } = new("Assets/audio/actor-footstep-soft-3.wav");
  public static SoundBiteSet ActorFootstepSoft  { get; } = new(ActorFootstepSoft1, ActorFootstepSoft2, ActorFootstepSoft3);

  public static SoundBite    ActorFootstepSolid1 { get; } = new("Assets/audio/actor-footstep-solid-1.wav");
  public static SoundBite    ActorFootstepSolid2 { get; } = new("Assets/audio/actor-footstep-solid-2.wav");
  public static SoundBite    ActorFootstepSolid3 { get; } = new("Assets/audio/actor-footstep-solid-3.wav");
  public static SoundBiteSet ActorFootstepSolid  { get; } = new(ActorFootstepSolid1, ActorFootstepSolid2, ActorFootstepSolid3);
}

public sealed class SoundBiteSet
{
  private readonly SoundBite[] bites;

  public SoundBiteSet(params SoundBite[] bites)
  {
    this.bites = bites;
  }

  public SoundBite SelectRandomly(Random random)
  {
    return bites.SelectRandomly(random);
  }
}

/// <summary>A top-level manager for audio playback.</summary>
public sealed class AudioManager : IDisposable
{
  private readonly LinkedList<Request> requests = new();

  private readonly IAssetManager assets;
  private readonly AudioSource[] audioSources;

  public AudioManager(IAudioServer server, IAssetManager assets, int maxVoices = 32)
  {
    this.assets = assets;

    audioSources = new AudioSource[maxVoices];

    for (var i = 0; i < maxVoices; i++)
    {
      audioSources[i] = new AudioSource(server);
    }
  }

  public void PlayBite(SoundBiteSet set)
  {
    PlayBite(set.SelectRandomly(Random.Shared));
  }

  private void PlayBite(SoundBite bite)
  {
    PlayClip(bite.Path);
  }

  public void PlayClip(VirtualPath path)
  {
    if (assets.TryGetAsset<AudioClip>(path, out var clip))
    {
      PlayClip(clip);
    }
    else
    {
      var task = assets.LoadAssetAsync<AudioClip>(path);

      requests.AddLast(new Request(path, task));
    }
  }

  public bool PlayClip(AudioClip clip)
  {
    for (var i = 0; i < audioSources.Length; i++)
    {
      if (!audioSources[i].IsPlaying)
      {
        audioSources[i].Play(clip);

        return true;
      }
    }

    return false;
  }

  public void Update()
  {
    for (var node = requests.First; node != null; node = node.Next)
    {
      var request = node.Value;
      if (request.IsReady)
      {
        PlayClip(request.Task.Result);

        requests.Remove(node);
      }
    }
  }

  public void Dispose()
  {
    foreach (var audioSource in audioSources)
    {
      audioSource.Dispose();
    }
  }

  private readonly record struct Request(VirtualPath Path, Task<AudioClip> Task)
  {
    public bool IsReady => Task.IsCompleted;
  }
}
