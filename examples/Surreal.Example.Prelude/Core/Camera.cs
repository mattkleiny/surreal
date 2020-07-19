using System.Numerics;

namespace Prelude.Core {
  public sealed class Camera {
    public Vector2 Position    { get; set; } = new Vector2(2, 2);
    public Vector2 Direction   { get; set; } = new Vector2(0, 1);
    public float   FocalLength { get; set; } = 1.0f;
    public float   WallHeight  { get; set; } = 1.0f;
    public float   FudgeBias   { get; set; } = 0.001f;
  }
}