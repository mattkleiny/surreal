using Surreal.Mathematics;

namespace Surreal.UI.Geometry;

/// <summary>The direction for clock winding.</summary>
public enum WindingDirection
{
  Clockwise,
  CounterClockwise,
}

/// <summary>Abstractly represents a segment for geometric path rendering.</summary>
public interface IPathSegment
{
}

/// <summary>A <see cref="IPathSegment"/> arc from a point around an angle.</summary>
public readonly record struct ArcSegment(Vector2 Point, Area Size, WindingDirection Winding) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> line between two points.</summary>
public readonly record struct LineSegment(Vector2 Point) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> with a quadratic bezier curve.</summary>
public readonly record struct QuadraticBezierSegment(Vector2 ControlPoint, Vector2 EndPoint) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> with a cubic bezier curve.</summary>
public readonly record struct CubicBezierSegment(Vector2 ControlPoint1, Vector2 ControlPoint2, Vector2 EndPoint) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> with multiple line pieces.</summary>
public readonly record struct PolyLineSegment(ImmutableArray<Vector2> Point) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> with multiple quadratic bezier pieces.</summary>
public readonly record struct PolyQuadraticBezierSegment(ImmutableArray<Vector2> Point) : IPathSegment;

/// <summary>A <see cref="IPathSegment"/> with multiple cubic bezier pieces.</summary>
public readonly record struct PolyCubicBezierSegment(ImmutableArray<Vector2> Point) : IPathSegment;

/// <summary>A builder for <see cref="VectorGeometry"/>.</summary>
public sealed class VectorGeometryBuilder
{
  public void AddSegment(ArcSegment segment) => throw new NotImplementedException();
  public void AddSegment(LineSegment segment) => throw new NotImplementedException();
  public void AddSegment(QuadraticBezierSegment segment) => throw new NotImplementedException();
  public void AddSegment(CubicBezierSegment segment) => throw new NotImplementedException();
  public void AddSegment(PolyLineSegment segment) => throw new NotImplementedException();
  public void AddSegment(PolyQuadraticBezierSegment segment) => throw new NotImplementedException();
  public void AddSegment(PolyCubicBezierSegment segment) => throw new NotImplementedException();
}

/// <summary>Represents built vector geometry, suitable for rendering.</summary>
public sealed class VectorGeometry
{
}
