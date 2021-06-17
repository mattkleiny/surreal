using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data;
using Surreal.Data.VFS;

namespace Surreal.Audio.Clips {
  public interface IAudioData {
    TimeSpan        Duration { get; }
    AudioSampleRate Rate     { get; }
    Size            Size     { get; }
    Span<byte>      Data     { get; }
  }

  public abstract class AudioClip : AudioResource, IHasSizeEstimate {
    private IAudioData? data;

    public TimeSpan Duration => data?.Duration ?? TimeSpan.MinValue;
    public Size     Size     => data?.Size ?? Size.Zero;

    public void Upload(IAudioData newData) {
      Upload(data, newData);
      data = newData;
    }

    protected abstract void Upload(IAudioData? existingData, IAudioData newData);

    public sealed class Loader : AssetLoader<AudioClip> {
      private readonly IAudioDevice device;

      public Loader(IAudioDevice device) {
        this.device = device;
      }

      public override async Task<AudioClip> LoadAsync(Path path, IAssetLoaderContext context) {
        var buffer = await context.GetAsync<AudioBuffer>(path);

        return device.CreateAudioClip(buffer);
      }
    }
  }
}