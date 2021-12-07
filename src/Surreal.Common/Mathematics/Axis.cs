namespace Surreal.Mathematics;

[Flags]
public enum Axis
{
  None       = 0,
  Vertical   = 1 << 0,
  Horizontal = 1 << 1,
  Everything = ~0
}