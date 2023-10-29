using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Utilities;

namespace Surreal.Audio;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Audio"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AudioModule(IAudioBackend backend) : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader>(new AudioBufferLoader());
    registry.AddService<IAssetLoader>(new AudioClipLoader(backend));
  }
}
