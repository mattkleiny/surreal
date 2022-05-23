using System.Runtime.InteropServices;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.UI.Immediate;

/// <summary>A key for <see cref="Style"/> in a <see cref="Skin"/>.</summary>
public readonly record struct StyleKey(string Name);

/// <summary>A single style in a <see cref="Skin"/>.</summary>
public sealed record Style
{
  public Color ForegroundColor { get; set; } = Color.White;
  public Color BackgroundColor { get; set; } = Color.Black;
  public Color ActiveColor     { get; set; } = Color.Black.Lighten(0.2f);
  public Color InactiveColor   { get; set; } = Color.White.Darken(0.2f);
  public float LineWidth       { get; set; } = 1f;
}

/// <summary>A skin for <see cref="ImmediateModeContext"/> rendering.</summary>
public sealed record Skin
{
  private readonly Dictionary<string, Style> stylesByName = new();

  /// <summary>The default <see cref="Style"/> to apply, if no other exists.</summary>
  public Style DefaultStyle { get; } = new();

  /// <summary>Attempts to get the <see cref="Style"/> with the given name.</summary>
  public Style GetStyleOrDefault(StyleKey key, Optional<Style> defaultStyle = default)
  {
    if (!stylesByName.TryGetValue(key.Name, out var style))
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
public sealed class ImmediateModeContext : IDisposable, IPaintingContext
{
  private readonly IPlatformHost host;
  private readonly Dictionary<int, object> state = new();

  private readonly IMouseDevice mouse;
  private readonly IKeyboardDevice keyboard;
  private readonly ShaderProgram shader;
  private readonly Mesh<Vertex> mesh;
  private readonly Tessellator<Vertex> tessellator;

  public ImmediateModeContext(IGraphicsServer graphics, IInputServer input, IPlatformHost host, IAssetManager assets)
  {
    this.host = host;

    mouse    = input.GetRequiredDevice<IMouseDevice>();
    keyboard = input.GetRequiredDevice<IKeyboardDevice>();

    shader      = assets.LoadAssetAsync<ShaderProgram>("resx://Surreal.UI/Resources/shaders/batched-ui.glsl").Result;
    mesh        = new Mesh<Vertex>(graphics);
    tessellator = mesh.CreateTessellator();
  }

  /// <summary>The active skin.</summary>
  public Skin Skin { get; set; } = new();

  public IMouseDevice    Mouse    => mouse;
  public IKeyboardDevice Keyboard => keyboard;

  /// <summary>Gets a unique ID for the control at the given position.</summary>
  public int GetControlId(Rectangle rectangle)
  {
    throw new NotImplementedException();
  }

  /// <summary>Gets the state for the given control.</summary>
  public TState GetState<TState>(int controlId)
    where TState : new()
  {
    return (TState)state.GetOrAdd(controlId);
  }

  /// <summary>Removes the state for the given control.</summary>
  public void ClearState(int controlId)
  {
    state.Remove(controlId);
  }

  /// <summary>Removes all state for all controls.</summary>
  public void ClearAllState()
  {
    state.Clear();
  }

  /// <summary>Renders the UI to the display.</summary>
  public void Present()
  {
    // update batch geometry
    tessellator.WriteTo(mesh);
    tessellator.Clear();

    // set-up a basic orthographic projection
    var projectionView =
      Matrix4x4.CreateTranslation(-host.Width / 2f, -host.Height / 2f, 0f) *
      Matrix4x4.CreateOrthographic(host.Width, host.Height, 0f, 100f);

    throw new NotImplementedException();
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

  public void DrawTriangleFan(ReadOnlySpan<Vector2> vertices, Color color)
  {
    var result = new SpanList<Vertex>(stackalloc Vertex[vertices.Length]);

    foreach (var position in vertices)
    {
      result.Add(new Vertex(position, color));
    }

    tessellator.AddTriangleFan(result);
  }

  public void DrawStrokeRect(Rectangle rectangle, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawFillRect(Rectangle rectangle, Color color)
  {
    tessellator.AddQuad(
      new Vertex(rectangle.BottomLeft, color),
      new Vertex(rectangle.TopLeft, color),
      new Vertex(rectangle.TopRight, color),
      new Vertex(rectangle.BottomRight, color)
    );
  }

  public void DrawText(Rectangle rectangle, string text, Color color)
  {
    throw new NotImplementedException();
  }

  public void DrawTexture(Rectangle rectangle, Texture texture)
  {
    throw new NotImplementedException();
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
    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 Position = Position;

    [VertexDescriptor(4, VertexType.Float)]
    public Color Color = Color;

    [VertexDescriptor(2, VertexType.Float)]
    public Vector2 UV = UV;

    [VertexDescriptor(1, VertexType.Float)]
    public float Thickness = Thickness;

    [VertexDescriptor(1, VertexType.Int)]
    public int ControlId = ControlId;

    [VertexDescriptor(1, VertexType.Int)]
    public int TextureIndex = TextureIndex;
  }
}
