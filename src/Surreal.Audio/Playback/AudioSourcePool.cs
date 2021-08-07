using System;
using System.Collections.Generic;
using System.Linq;

namespace Surreal.Audio.Playback
{
/// <summary>Represents a pool of <see cref="AudioSource"/>s.</summary>
  public sealed class AudioSourcePool : IDisposable
  {
    private readonly AudioSource[] sources;

    public AudioSourcePool(IEnumerable<AudioSource> sources)
    {
      this.sources = sources.ToArray();
    }

    public AudioSource? GetAudioSource()
    {
      for (var i = 0; i < sources.Length; i++)
      {
        if (!sources[i].IsPlaying)
        {
          return sources[i];
        }
      }

      return null;
    }

    public void Dispose()
    {
      foreach (var source in sources)
      {
        source.Dispose();
      }
    }
  }
}
