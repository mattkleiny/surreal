using System.Runtime.InteropServices;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.UI.Immediate;

/// <summary>A single style in a <see cref="Skin"/>.</summary>
public sealed record Style
{
  public Color ForegroundColor { get; set; } = Color.White;
  public Color BackgroundColor { get; set; } = Color.Black;
  public float LineWidth       { get; set; } = 1f;
}

/// <summary>A skin for <see cref="ImmediateModeContext"/> rendering.</summary>
public sealed record Skin
{
  private readonly Dictionary<string, Style> stylesByName = new();

  /// <summary>The default <see cref="Style"/> to apply, if no other exists.</summary>
  public Style DefaultStyle { get; } = new();

  /// <summary>Attempts to get the <see cref="Style"/> with the given name.</summary>
  public Style GetStyleOrDefault(string name, Optional<Style> defaultStyle = default)
  {
    if (!stylesByName.TryGetValue(name, out var style))
    {
      return defaultStyle.GetOrDefault(DefaultStyle);
    }

    return style;
  }

  /// <summary>Sets the <see cref="Style"/> with the given name.</summary>
  public void SetStyle(string name, Style style)
  {
    stylesByName[name] = style;
  }
}

/// <summary>A context for immediate mode rendering.</summary>
public sealed class ImmediateModeContext : IDisposable
{
  private readonly Dictionary<int, object> state = new();

  public ImmediateModeContext(IImmediateModeRenderer renderer)
  {
    Renderer = renderer;
  }

  /// <summary>The active immediate mode skin.</summary>
  public Skin Skin { get; set; } = new();

  /// <summary>The active <see cref="IImmediateModeRenderer"/>.</summary>
  public IImmediateModeRenderer Renderer { get; }

  /// <summary>Gets a unique ID for the control at the given position.</summary>
  public int GetControlId(Rectangle rectangle)
  {
    throw new NotImplementedException();
  }

  /// <summary>Gets the state for the given control.</summary>
  public TState GetState<TState>(int controlId)
    where TState : new()
  {
    return (TState) state.GetOrAdd(controlId);
  }

  /// <summary>Removes the state for the given control.</summary>
  public void ClearState(int controlId)
  {
    state.Remove(controlId);
  }

  public void Update(TimeDelta deltaTime)
  {
  }

  public void Draw(TimeDelta deltaTime)
  {
    Renderer.Flush();
  }

  public void Dispose()
  {
    Renderer.Dispose();
  }
}

/// <summary>Allows an <see cref="ImmediateModeContext"/> to render to some display.</summary>
public interface IImmediateModeRenderer : IDisposable
{
  void DrawLine(Vector2 from, Vector2 to, Color color, float thickness = 1f);
  void DrawLineStrip(ReadOnlySpan<Vector2> vertices, Color color, float thickness = 1f);
  void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color);
  void DrawTriangleStrip(ReadOnlySpan<Vector2> vertices, Color color);
  void DrawStrokeRect(Rectangle rectangle, Color color);
  void DrawFillRect(Rectangle rectangle, Color color);
  void DrawText(Rectangle rectangle, string text, Color color);
  void DrawTexture(Rectangle rectangle, Texture texture);

  /// <summary>Flushes all commands to the underlying hardware.</summary>
  void Flush();
}

/// <summary>An <see cref="IImmediateModeRenderer"/> that uses a simple geometry batch and specialized shader program.</summary>
public sealed class BatchedImmediateModeRenderer : IImmediateModeRenderer
{
  private readonly ShaderProgram shader;
  private readonly Mesh<Vertex> mesh;
  private readonly Tessellator<Vertex> tessellator;

  public static async ValueTask<BatchedImmediateModeRenderer> CreateAsync(IGraphicsServer server, IAssetManager assets)
  {
    var shader = await assets.LoadAssetAsync<ShaderProgram>("resx://Surreal.UI/Resources/shaders/batchedui.glsl");

    return new BatchedImmediateModeRenderer(server, shader);
  }

  private BatchedImmediateModeRenderer(IGraphicsServer server, ShaderProgram shader)
  {
    this.shader = shader;

    mesh        = new Mesh<Vertex>(server);
    tessellator = mesh.CreateTessellator();
  }

  public void DrawLine(Vector2 from, Vector2 to, Color color, float thickness = 1)
  {
    tessellator.AddLine(
      new Vertex(from, color, Thickness: thickness),
      new Vertex(to, color, Thickness: thickness)
    );
  }

  public void DrawLineStrip(ReadOnlySpan<Vector2> vertices, Color color, float thickness = 1)
  {
    throw new NotImplementedException();
  }

  public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
  {
    tessellator.AddTriangle(
      new Vertex(a, color),
      new Vertex(b, color),
      new Vertex(c, color)
    );
  }

  public void DrawTriangleStrip(ReadOnlySpan<Vector2> vertices, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawStrokeRect(Rectangle rectangle, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawFillRect(Rectangle rectangle, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawText(Rectangle rectangle, string text, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawTexture(Rectangle rectangle, Texture texture)
  {
    throw new NotImplementedException();
  }

  public void Flush()
  {
    tessellator.WriteTo(mesh);
    mesh.Draw(shader);
    tessellator.Clear();
  }

  public void Dispose()
  {
    mesh.Dispose();
    shader.Dispose();
  }

  /// <summary>A vertex used in immediate mode rendering.</summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex(Vector2 Position, Color Color, Vector2 UV = default, float Thickness = 1f, int ControlId = -1, int TextureIndex = -1)
  {
    [VertexDescriptor(VertexType.Float, 2)]
    public Vector2 Position = Position;

    [VertexDescriptor(VertexType.Float, 4)]
    public Color Color = Color;

    [VertexDescriptor(VertexType.Float, 2)]
    public Vector2 UV = UV;

    [VertexDescriptor(VertexType.Float, 1)]
    public float Thickness = Thickness;

    [VertexDescriptor(VertexType.Int, 1)]
    public int ControlId = ControlId;

    [VertexDescriptor(VertexType.Int, 1)]
    public int TextureIndex = TextureIndex;
  }
}
