using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Rendering;
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
  /// The <see cref="IRenderPipeline"/> to use for the scene.
  /// </summary>
  public IRenderPipeline? Renderer { get; init; }

  /// <inheritdoc/>
  public void Render(DeltaTime deltaTime)
  {
    Renderer?.Render(this, deltaTime);
  }

  /// <inheritdoc/>
  public ReadOnlySlice<IRenderViewport> CullVisibleViewports()
  {
    return ResolveChildren<IRenderViewport>();
  }

  protected override void OnPostUpdate(DeltaTime deltaTime)
  {
    base.OnPostUpdate(deltaTime);

    while (MessagesForParents.TryDequeue(out var message))
    {
      switch (message)
      {
        case { Type: MessageType.Destroy, Sender: var sender }:
        {
          sender.Parent?.Children.Remove(sender);
          sender.DestroyIfNecessary();

          break;
        }
      }
    }
  }
}
