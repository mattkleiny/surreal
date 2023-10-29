using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Utilities;

namespace Surreal.Audio;

/// <summary>
/// A <see cref="IServiceModule"/> for the audio system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AudioModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    var backend = registry.GetServiceOrThrow<IAudioBackend>();

    registry.AddService<IAssetLoader>(new AudioBufferLoader());
    registry.AddService<IAssetLoader>(new AudioClipLoader(backend));
  }
}
