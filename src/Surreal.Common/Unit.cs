namespace Surreal;

/// <summary>A no-op void type.</summary>
public readonly record struct Unit
{
  public static Unit Default => default;

  public override string ToString() => "()";
}
