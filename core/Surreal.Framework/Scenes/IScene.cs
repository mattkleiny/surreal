﻿using Surreal.Graphics.Rendering;
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
