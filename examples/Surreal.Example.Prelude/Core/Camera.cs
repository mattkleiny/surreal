using System.Numerics;

namespace Prelude.Core {
  public sealed class Camera {
    public Vector2 Position    { get; set; } = new(2, 2);
    public Vector2 Direction   { get; set; } = new(0, 1);
    public float   FocalLength { get; set; } = 1.0f;
    public float   WallHeight  { get; set; } = 1.0f;
    public float   FudgeBias   { get; set; } = 0.001f;
  }
}