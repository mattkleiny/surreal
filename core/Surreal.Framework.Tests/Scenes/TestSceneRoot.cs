using Surreal.Assets;
using Surreal.Graphics.Rendering;
using Surreal.Physics;

namespace Surreal.Scenes;

internal sealed class TestSceneRoot : ISceneRoot
{
  public IAssetProvider Assets { get; init; } = Substitute.For<IAssetProvider>();
  public IServiceProvider Services { get; init; } = Substitute.For<IServiceProvider>();
  public IRenderPipeline? Renderer { get; set; }
  public IPhysicsWorld? Physics { get; set; }
}
