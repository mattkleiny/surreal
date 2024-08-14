namespace Surreal.Graphics;

/// <summary>
/// Performance mode for the graphics backend.
/// </summary>
public enum PerformanceMode
{
  /// <summary>
  /// A universal performance mode that should work on all devices.
  /// </summary>
  Universal,
  /// <summary>
  /// A high-definition performance mode that should work on high-end devices.
  /// </summary>
  HighDefinition,
}

/// <summary>
/// A descriptor for a graphics device.
/// </summary>
public record struct GraphicsDeviceDescriptor
{
  /// <summary>
  /// The desired performance mode for the graphics device.
  /// <para/>
  /// This might be used to select a different rendering pipeline or set of shaders
  /// depending on the underlying implementation.
  /// </summary>
  public required PerformanceMode Mode { get; set; }
}

/// <summary>
/// An abstraction over the different types of graphics backends available.
/// </summary>
public interface IGraphicsBackend
{
  static IGraphicsBackend Null { get; } = new NullGraphicsBackend();

  /// <summary>
  /// Creates a new <see cref="IGraphicsDevice"/>.
  /// </summary>
  IGraphicsDevice CreateDevice(GraphicsDeviceDescriptor descriptor);

  /// <summary>
  /// A no-op <see cref="IGraphicsBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullGraphicsBackend : IGraphicsBackend
  {
    public IGraphicsDevice CreateDevice(GraphicsDeviceDescriptor descriptor)
    {
      return IGraphicsDevice.Null;
    }
  }
}
