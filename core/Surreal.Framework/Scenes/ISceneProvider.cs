namespace Surreal.Scenes;

/// <summary>
/// A provider of scenes.
/// </summary>
public interface ISceneProvider
{
  /// <summary>
  /// The currently active scene, or null.
  /// </summary>
  IScene? CurrentScene { get; }
}
