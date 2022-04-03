namespace Surreal.Mathematics;

/// <summary>A bounding box in 2-space.</summary>
public readonly record struct BoundingRect(Vector2 Min, Vector2 Max)
{
  public float X => Min.X;
  public float Y => Min.Y;

  public float Width  => Max.X - Min.X;
  public float Height => Max.Y - Min.Y;

  public bool Contains(Point2 point)
  {
    return point.X >= Min.X &&
           point.X <= Max.X &&
           point.Y >= Min.Y &&
           point.Y <= Max.Y;
  }

  public bool Contains(Vector2 vector)
  {
    return vector.X >= Min.X &&
           vector.X <= Max.X &&
           vector.Y >= Min.Y &&
           vector.Y <= Max.Y;
  }
}

/// <summary>A bounding box in 3-space.</summary>
public readonly record struct BoundingBox(Vector3 Min, Vector3 Max)
{
  public float X => Min.X;
  public float Y => Min.Y;
  public float Z => Min.Z;

  public float Width  => Max.X - Min.X;
  public float Height => Max.Y - Min.Y;
  public float Depth  => Max.Z - Min.Z;

  public bool Contains(Point3 point)
  {
    return point.X >= Min.X &&
           point.X <= Max.X &&
           point.Y >= Min.Y &&
           point.Y <= Max.Y &&
           point.Z >= Min.Z &&
           point.Z <= Max.Z;
  }

  public bool Contains(Vector3 vector)
  {
    return vector.X >= Min.X &&
           vector.X <= Max.X &&
           vector.Y >= Min.Y &&
           vector.Y <= Max.Y &&
           vector.Z >= Min.Z &&
           vector.Z <= Max.Z;
  }
}
