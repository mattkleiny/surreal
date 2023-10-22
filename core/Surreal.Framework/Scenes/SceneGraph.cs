using Surreal.Collections;
using Surreal.Graphics.Rendering;
using Surreal.Timing;
using static Surreal.Scenes.SceneNode;

namespace Surreal.Scenes;

/// <summary>
/// A scene that uses <see cref="SceneNode"/>s as it's core building block.
/// </summary>
public sealed class SceneGraph : IScene, IDisposable
{
  /// <summary>
  /// The root <see cref="SceneNode"/> used by this scene.
  /// </summary>
  public SceneNode Root { get; } = new();

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderCamera> CullVisibleCameras()
  {
    return Root.ResolveChildren<IRenderCamera>();
  }

  /// <inheritdoc/>
  public void Update(DeltaTime deltaTime)
  {
    Root.Update(deltaTime);

    // process messages
    while (Root.Outbox.TryDequeue(out var notification))
    {
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

  public void Dispose()
  {
    Root.Dispose();
  }
}
