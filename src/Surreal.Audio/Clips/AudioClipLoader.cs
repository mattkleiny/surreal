using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Audio.Clips
{
  /// <summary>The <see cref="AssetLoader{T}"/> for <see cref="AudioClip"/>s.</summary>
  public sealed class AudioClipLoader : AssetLoader<AudioClip>
  {
    private readonly IAudioDevice device;

    public AudioClipLoader(IAudioDevice device)
    {
      this.device = device;
    }

    public override async Task<AudioClip> LoadAsync(Path path, IAssetResolver resolver)
    {
      var buffer = await resolver.LoadAsset<AudioBuffer>(path);

      return device.CreateAudioClip(buffer.Data);
    }
  }
}
