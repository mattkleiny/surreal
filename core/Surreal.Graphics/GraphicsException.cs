namespace Surreal.Graphics;

/// <summary>
/// Represents an error in the graphics system.
/// </summary>
public class GraphicsException(string? message, Exception? innerException = null) : ApplicationException(message, innerException);
