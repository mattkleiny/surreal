using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Terminals
{
  /// <summary>Renders text in an old-school terminal style using bitmap fonts.</summary>
  public class Terminal : IDisposable
  {
    private readonly Mesh<Vertex> mesh;

    public Terminal(IGraphicsDevice device, BitmapFont font, IntSize displaySize, IntSize characterSize, float scale = 1f)
    {
      Font          = font;
      DisplaySize   = displaySize;
      CharacterSize = characterSize;
      Scale         = scale;

      mesh = new Mesh<Vertex>(device);
    }

    public BitmapFont Font          { get; }
    public IntSize    DisplaySize   { get; }
    public IntSize    CharacterSize { get; }
    public float      Scale         { get; }

    public void Dispose()
    {
      mesh.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex
    {
      [VertexDescriptor(
          Alias = "a_position",
          Count = 2,
          Type = VertexType.Float
      )]
      public Vector2 Position;

      [VertexDescriptor(
          Alias = "a_color",
          Count = 4,
          Type = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;

      public Vertex(Vector2 position, Color color)
      {
        Position = position;
        Color    = color;
      }
    }
  }
}
