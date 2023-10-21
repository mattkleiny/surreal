using Surreal.Utilities;

namespace Surreal.Graphics;

/// <summary>
/// The default <see cref="GraphicsContext"/> implementation.
/// </summary>
public readonly record struct GraphicsContext(IGraphicsBackend Backend)
{
  /// <summary>
  /// Creates a new <see cref="GraphicsContext"/> with the default <see cref="IGraphicsBackend"/>.
  /// </summary>
  public static GraphicsContext Default => new(Game.Services.GetServiceOrThrow<IGraphicsBackend>());

  /// <summary>
  /// Creates a new <see cref="GraphicsContext"/> with a <see cref="HeadlessGraphicsBackend"/>.
  /// </summary>
  public static GraphicsContext Headless => new(new HeadlessGraphicsBackend());
}
