using Surreal.Assets;
using Surreal.Collections.Slices;
using Surreal.Graphics.Rendering;
using Surreal.Physics;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="IScene"/> that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneTree : SceneNode, IScene, ISceneRoot, ISceneProvider
{
  IScene ISceneProvider.CurrentScene => this;

  /// <inheritdoc/>
  public new required IAssetProvider Assets { get; init; }

  /// <inheritdoc/>
  public new required IServiceProvider Services { get; init; }

  /// <inheritdoc/>
  public IPhysicsWorld? Physics { get; set; }

  /// <summary>
  /// Resets the tree and removes all of its children.
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
  internal override bool TryResolveRoot(out ISceneRoot result)
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
