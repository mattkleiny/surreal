using System;
using System.Numerics;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;

namespace Silence.Core.Sprites
{
  public class GeometrySprite
  {
    public PrimitiveType Type   { get; }
    public Vector2[]     Points { get; }
    public Color         Color  { get; set; } = Color.White;

    public GeometrySprite(ReadOnlySpan<Vector2> points, PrimitiveType type)
    {
      Type   = type;
      Points = points.ToArray();
    }

    public void Draw(GeometryBatch batch)
    {
      batch.DrawPrimitive(Points, Color, Type);
    }
  }
}
