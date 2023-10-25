using Surreal.Assets;
using Surreal.Collections;
using Surreal.Diagnostics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Physics;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="IScene"/> that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneTree : SceneNode, IScene, IGizmoObject
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

  /// <summary>
  /// Resets the tree and removes all of it's children.
  /// </summary>
  public void Reset()
  {
    PhysicsWorld?.Reset();
    Children.Clear();
  }

  /// <inheritdoc/>
  public new void Update(DeltaTime deltaTime)
  {
    PhysicsWorld?.Tick(deltaTime);

    base.Update(deltaTime);
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

  /// <inheritdoc/>
  void IGizmoObject.RenderGizmos(IGizmoBatch gizmos)
  {
    if (PhysicsWorld is IGizmoObject physicsWorld)
    {
      physicsWorld.RenderGizmos(gizmos);
    }
  }

  internal override void OnMessageReceivedFromChild(Message message)
  {
    base.OnMessageReceivedFromChild(message);

    switch (message)
    {
      case { Type: MessageType.Destroy, Sender: var sender }:
      {
        sender.OnDestroyIfNecessary();

        break;
      }
    }
  }
}
