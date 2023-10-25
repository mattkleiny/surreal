using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="IScene"/> that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneTree(ISceneRoot root) : SceneNode, IScene
{
  /// <inheritdoc/>
  public new void Update(DeltaTime deltaTime)
  {
    root.Physics?.Tick(deltaTime);

    base.Update(deltaTime);
  }

  /// <inheritdoc/>
  public void Render(DeltaTime deltaTime)
  {
    root.Renderer?.Render(this, deltaTime);
  }

  /// <summary>
  /// Resets the tree and removes all of it's children.
  /// </summary>
  public void Reset()
  {
    root.Physics?.Reset();
    Children.Clear();
  }

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderViewport> CullActiveViewports()
  {
    return ResolveChildren<IRenderViewport>();
  }

  /// <inheritdoc/>
  protected override bool TryResolveRoot(out ISceneRoot result)
  {
    result = root;
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
