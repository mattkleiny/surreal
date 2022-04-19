﻿namespace Surreal.Graphics;

/// <summary>Represents an error in the graphics system.</summary>
public class GraphicsException : SurrealException
{
  public GraphicsException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}