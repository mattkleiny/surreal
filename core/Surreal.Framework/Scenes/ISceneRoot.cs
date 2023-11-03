using Surreal.Assets;
using Surreal.Graphics.Rendering;
using Surreal.Physics;

namespace Surreal.Scenes;

/// <summary>
/// The root of a scene.
/// <para/>
/// Provides services and assets to child nodes for access to game context.
/// </summary>
public interface ISceneRoot
{
  /// <summary>
  /// The assets to provide to the scene.
  /// </summary>
  IAssetProvider Assets { get; }

  /// <summary>
  /// The services to provide to the scene.
  /// </summary>
  IServiceProvider Services { get; }

  /// <summary>
  /// The <see cref="IRenderPipeline"/> connected to the scene.
  /// </summary>
  IRenderPipeline? Renderer { get; }

  /// <summary>
  /// The <see cref="IPhysicsWorld"/> connected to the scene.
  /// </summary>
  IPhysicsWorld? Physics { get; }
}
