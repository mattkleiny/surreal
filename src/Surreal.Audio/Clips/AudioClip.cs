using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips {
  public abstract class AudioClip : AudioResource, IHasSizeEstimate {
    private IAudioData? data;

    public TimeSpan Duration  => data?.Duration   ?? TimeSpan.MinValue;
    public int      Frequency => data?.SampleRate ?? 0;
    public Size     Size      => data?.Size       ?? Size.Zero;

    public void Upload(IAudioData data) {
      Upload(this.data, data);

      this.data = data;
    }

    protected abstract void Upload(IAudioData? existingData, IAudioData newData);

    public sealed class Loader : AssetLoader<AudioClip> {
      private readonly IAudioDevice device;

      public Loader(IAudioDevice device) {
        this.device = device;
      }

      public override async Task<AudioClip> LoadAsync(Path path, IAssetLoaderContext context) {
        var waveform = await context.GetAsync<WaveData>(path);

        return device.Factory.CreateAudioClip(waveform);
      }
    }
  }
}