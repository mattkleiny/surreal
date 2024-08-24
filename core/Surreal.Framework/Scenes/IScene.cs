using Surreal.Assets;
using Surreal.Graphics.Rendering;
using Surreal.Physics;

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
  /// Sends an event to the scene.
  /// </summary>
  void Publish<T>(T @event)
    where T : ISceneEvent;
}

/// <summary>
/// Describes a scene, allowing it to be instantiated and loaded.
/// </summary>
public interface ISceneDefinition;

/// <summary>
/// The root of a scene.
/// <para/>
/// Provides services and assets to child nodes for access to game context.
/// </summary>
internal interface ISceneRoot
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
  /// The <see cref="IPhysicsWorld"/> connected to the scene.
  /// </summary>
  IPhysicsWorld? Physics { get; }
}
