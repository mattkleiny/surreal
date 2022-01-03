namespace Surreal.Mathematics;

/// <summary>Axis combinations in euclidean space.</summary>
[Flags]
public enum Axis
{
  None       = 0,
  Vertical   = 1 << 0,
  Horizontal = 1 << 1,
  All        = Vertical | Horizontal,
}
