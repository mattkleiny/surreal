namespace Surreal.Resources;

/// <summary>
/// A resource that can be loaded from the game's <see cref="IResourceProvider"/>.
/// </summary>
public interface IResource
{
  /// <summary>
  /// The unique name of this resource.
  /// </summary>
  string ResourceName { get; }
}
