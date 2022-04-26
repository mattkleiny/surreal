using Surreal.Graphics.Shaders;

namespace Surreal.Graphics.UI;

/// <summary>An immediate mode UI context for graphics rendering.</summary>
public sealed class ImmediateModeCanvas : IDisposable
{
  private readonly IGraphicsServer server;
  private readonly ShaderProgram shader;

  public ImmediateModeCanvas(IGraphicsServer server, ShaderProgram shader)
  {
    this.server = server;
    this.shader = shader;
  }

  public void Dispose()
  {
  }
}

/// <summary>Utility extensions for <see cref="ImmediateModeCanvas"/> rendering.</summary>
public static class ImmediateModeCanvasExtensions
{
}
