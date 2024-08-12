using Surreal.Assets;
using Surreal.Graphics.Rendering;
using Surreal.Physics;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A scene is a collection of objects that are updated and rendered together.
/// <para/>
/// The exact definition of a scene is up to the application.
/// For example, a scene could be a level in a game, a menu, or a cutscene, etc.
/// <para/>
/// The mechanisms used to update and render the scene are also up to the application,
/// so a scene could be a Scene Graph or an Actor Model or an Entity Component System,
/// etc.
/// </summary>
public interface IScene : IRenderScene
{
  /// <summary>
  /// Updates the scene.
  /// </summary>
  void Update(DeltaTime deltaTime);

  /// <summary>
  /// Renders the scene.
  /// </summary>
  void Render(DeltaTime deltaTime);
}

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

/// <summary>
/// Describes a scene, allowing it to be instantiated and loaded.
/// </summary>
public interface ISceneDefinition;
