using System.Numerics;

namespace Surreal.Graphics.Raycasting {
  public sealed class RaycastCamera {
    public Vector2 Position    { get; set; } = new Vector2(2, 2);
    public Vector2 Direction   { get; set; } = new Vector2(0, 1);
    public float   FocalLength { get; set; } = 1.0f;
    public float   WallHeight  { get; set; } = 1.0f;
    public float   FudgeBias   { get; set; } = 0.001f;
  }
}