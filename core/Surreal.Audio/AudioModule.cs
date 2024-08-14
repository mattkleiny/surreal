using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Services;

namespace Surreal.Audio;

/// <summary>
/// A <see cref="IServiceModule"/> for the audio system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AudioModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader, AudioBufferLoader>();
    registry.AddService<IAssetLoader, AudioClipLoader>();
  }
}
