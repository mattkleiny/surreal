namespace Surreal.Mathematics
{
  /// <summary>Represents a normalized pivot point.</summary>
  public readonly record struct Pivot(Normal X, Normal Y)
  {
    public static readonly Pivot Min    = new(0f, 0f);
    public static readonly Pivot Max    = new(1f, 1f);
    public static readonly Pivot Center = new(0.5f, 0.5f);

    public override string ToString() => $"Pivot around <{X.ToString()} {Y.ToString()}>";
  }
}
