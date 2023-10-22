using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A scene that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneGraph : SceneNode, IScene
{
  /// <summary>
  /// The top-level <see cref="IAssetProvider"/> available to the scene.
  /// </summary>
  public new required IAssetProvider Assets { get; init; }

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> available to the scene.
  /// </summary>
  public new required IServiceProvider Services { get; init; }

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderViewport> CullVisibleViewports()
  {
    return ResolveChildren<IRenderViewport>();
  }

  protected override void OnPostUpdate(DeltaTime deltaTime)
  {
    base.OnPostUpdate(deltaTime);

    // process top-level messages
    while (Inbox.TryDequeue(out var notification))
    {
      // handle root notifications
      switch (notification)
      {
        case { Type: NotificationType.Destroy, Sender: var sender }:
        {
          sender.Parent?.Children.Remove(sender);
          sender.NotifyDestroyed();

          break;
        }
      }
    }
  }
}
