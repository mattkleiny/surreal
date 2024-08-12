﻿using Surreal.Assets;
using Surreal.Collections.Slices;
using Surreal.Graphics.Rendering;
using Surreal.Physics;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="IScene"/> that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneTree : SceneNode, IScene, ISceneRoot
{
  /// <inheritdoc/>
  public new required IAssetProvider Assets { get; init; }

  /// <inheritdoc/>
  public new required IServiceProvider Services { get; init; }

  /// <inheritdoc/>
  public IRenderPipeline? Renderer { get; set; }

  /// <inheritdoc/>
  public IPhysicsWorld? Physics { get; set; }

  /// <inheritdoc/>
  public new void Update(DeltaTime deltaTime)
  {
    Physics?.Tick(deltaTime);

    base.Update(deltaTime);
  }

  /// <inheritdoc/>
  public void Render(DeltaTime deltaTime)
  {
    Renderer?.Render(this, deltaTime);
  }

  /// <summary>
  /// Resets the tree and removes all of it's children.
  /// </summary>
  public void Reset()
  {
    Children.Clear();
    Physics?.Reset();
  }

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderViewport> CullActiveViewports()
  {
    return ResolveChildren<IRenderViewport>();
  }

  /// <inheritdoc/>
  protected override bool TryResolveRoot(out ISceneRoot result)
  {
    result = this;
    return true;
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

/// <summary>
/// A <see cref="ISceneDefinition"/> for a <see cref="SceneTree"/>.
/// <para/>
/// Allows a <see cref="SceneTree"/> to be instantiated and loaded at runtime,
/// and permits its use inside the Editor and Debugging tools for introspection.
/// </summary>
[XmlRoot("SceneTree")]
public sealed class SceneTreeDefinition : SceneNodeDefinition, ISceneDefinition
{
  /// <summary>
  /// Builds a new scene graph and attaches it as a child of the given parent node.
  /// </summary>
  public void BuildSubTree(SceneNode parent)
  {
    // TODO: implement me
  }
}
