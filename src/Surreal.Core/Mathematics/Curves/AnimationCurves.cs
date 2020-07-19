namespace Surreal.Mathematics.Curves {
  public delegate float AnimationCurve(Normal time);

  public static class AnimationCurves {
    public static AnimationCurve Constant(float d) => _ => d;

    public static AnimationCurve PlanarX<T>(T curve)
        where T : IPlanarCurve => time => curve.SampleAt(time).X;

    public static AnimationCurve PlanarY<T>(T curve)
        where T : IPlanarCurve => time => curve.SampleAt(time).Y;

    public static AnimationCurve Linear        { get; } = time => time;
    public static AnimationCurve InverseLinear { get; } = time => 1f - time;
  }
}