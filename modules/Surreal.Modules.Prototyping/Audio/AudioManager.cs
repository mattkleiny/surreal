using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Collections;
using Surreal.IO;

namespace Surreal.Audio;

/// <summary>A reference to a sound at a particular path.</summary>
public readonly record struct SoundBite(VirtualPath Path);

/// <summary>A set of many <see cref="SoundBite"/>s that are equivalent and weighted.</summary>
public sealed class SoundBiteSet : IEnumerable<SoundBite>
{
  private readonly WeightedList<SoundBite> bites = new();

  public void Add(SoundBite bite, float weight = 1f)
  {
    bites.Add(bite, weight);
  }

  public bool TrySelectWeighted(Random random, out SoundBite result)
  {
    return bites.TrySelect(random, out result);
  }

  public IEnumerator<SoundBite> GetEnumerator()
  {
    return bites.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}

/// <summary>A top-level manager for audio playback.</summary>
public sealed class AudioManager : IDisposable
{
  private readonly LinkedList<Task<AudioClip>> requests = new();

  private readonly IAssetManager assets;
  private readonly AudioSource[] audioSources;

  public AudioManager(IAudioServer server, IAssetManager assets, int maxSources = 32)
  {
    this.assets = assets;

    audioSources = new AudioSource[maxSources];

    for (var i = 0; i < maxSources; i++)
    {
      audioSources[i] = new AudioSource(server);
    }
  }

  public void PlayBite(SoundBiteSet set)
  {
    if (set.TrySelectWeighted(Random.Shared, out var bite))
    {
      PlayBite(bite);
    }
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
      requests.AddLast(assets.LoadAssetAsync<AudioClip>(path));
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
      if (request.IsCompleted)
      {
        PlayClip(request.Result);

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
}
