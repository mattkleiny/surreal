using Surreal.Timing;
using static Surreal.Scenes.SceneGraphNode;

namespace Surreal.Scenes;

/// <summary>
/// A scene that uses a Scene Graph to update and render its objects.
/// </summary>
public sealed class SceneGraphScene : IScene
{
  /// <summary>
  /// The root <see cref="SceneGraphNode"/> used by this scene.
  /// </summary>
  public SceneGraphNode Root { get; } = new();

  public void Tick(DeltaTime deltaTime)
  {
    Root.Update(deltaTime);
    Root.Render(deltaTime);

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
