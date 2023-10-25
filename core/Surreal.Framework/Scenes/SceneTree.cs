using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Physics;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="IScene"/> that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneTree : SceneNode, IScene
{
  /// <summary>
  /// The top-level <see cref="IAssetProvider"/> available to the scene.
  /// </summary>
  public new required IAssetProvider Assets { get; init; }

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> available to the scene.
  /// </summary>
  public new required IServiceProvider Services { get; init; }

  /// <summary>
  /// The <see cref="IRenderPipeline"/> attached to the scene.
  /// </summary>
  public IRenderPipeline? RenderPipeline { get; init; }

  /// <summary>
  /// The <see cref="IPhysicsWorld"/> attached to the scene.
  /// </summary>
  public IPhysicsWorld? PhysicsWorld { get; init; }

  /// <inheritdoc/>
  public void Update(DeltaTime deltaTime)
  {
    OnUpdate(deltaTime);

    PhysicsWorld?.Tick(deltaTime);
  }

  /// <inheritdoc/>
  public void Render(DeltaTime deltaTime)
  {
    RenderPipeline?.Render(this, deltaTime);
  }

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderViewport> CullActiveViewports()
  {
    return ResolveChildren<IRenderViewport>();
  }
}
