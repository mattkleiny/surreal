using System.Runtime.InteropServices;
using Surreal.Collections;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.UI.Immediate;

/// <summary>A skin for <see cref="ImmediateModeContext"/> rendering.</summary>
public sealed record Skin
{
  public static Skin Default { get; } = new();

  public Style DefaultStyle { get; } = new();
}

/// <summary>A single style in a <see cref="Skin"/>.</summary>
public sealed record Style
{
  public Color ForegroundColor { get; set; } = Color.White;
  public Color BackgroundColor { get; set; } = Color.Black;
  public float LineWidth       { get; set; } = 1f;
}

/// <summary>A single input event in an <see cref="ImmediateModeContext"/>.</summary>
public readonly record struct InputEvent
{
  public Vector2 Position { get; init; }
  public Key     Key      { get; init; }
}

/// <summary>A context for immediate mode rendering.</summary>
public sealed class ImmediateModeContext : IDisposable
{
  private readonly ShaderProgram shader;
  private readonly Mesh<Vertex> mesh;
  private readonly Dictionary<int, object> state = new();

  public ImmediateModeContext(IGraphicsServer server, ShaderProgram shader)
  {
    this.shader = shader;

    mesh = new Mesh<Vertex>(server);
  }

  /// <summary>The active immediate mode skin.</summary>
  public Skin Skin { get; set; } = Skin.Default;

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

  public void DrawBox(Rectangle rectangle)
  {
    throw new NotImplementedException();
  }

  public void DrawLabel(Rectangle position, string label)
  {
    DrawLabel(position, label, Skin.DefaultStyle);
  }

  public void DrawLabel(Rectangle position, string label, Style style)
  {
    throw new NotImplementedException();
  }

  public void DrawTexture(Rectangle rectangle, Texture texture)
  {
    DrawTexture(rectangle, texture, Skin.DefaultStyle);
  }

  public void DrawTexture(Rectangle rectangle, Texture texture, Style style)
  {
    throw new NotImplementedException();
  }

  public void Update(TimeDelta deltaTime)
  {
  }

  public void Draw(TimeDelta deltaTime)
  {
    shader.SetUniform("u_projectionView", Matrix4x4.Identity);

    mesh.Draw(shader);
  }

  public void Dispose()
  {
    mesh.Dispose();
  }

  /// <summary>A vertex used in immediate mode rendering.</summary>
  [StructLayout(LayoutKind.Sequential)]
  private record struct Vertex(Vector2 Position, Color Color, Vector2 UV, int TextureIndex = -1)
  {
    [VertexDescriptor(VertexType.Float, 2)]
    public Vector2 Position = Position;

    [VertexDescriptor(VertexType.Float, 4)]
    public Color Color = Color;

    [VertexDescriptor(VertexType.Float, 2)]
    public Vector2 UV = UV;

    [VertexDescriptor(VertexType.Int, 1)]
    public int TextureIndex = TextureIndex;
  }
}
