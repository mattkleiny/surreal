namespace RayTracer.Scenes;

public record struct Ray(Vector3 Origin, Vector3 Direction)
{
  public static Ray Reflect(Vector3 normal, Vector3 incidence, Vector3 intersection, float bias)
  {
    var origin = intersection + normal * bias;
    var direction = incidence - normal * 2.0f * Vector3.Dot(incidence, normal);

    return new Ray(origin, direction);
  }
}

/// <summary>A scene that can be ray-traced</summary>
public sealed class Scene
{
  public float      FieldOfView { get; set; } = 90.0f;
  public Color32    Background  { get; set; } = Color32.Black;
  public int        Width       { get; set; }
  public int        Height      { get; set; }
  public List<Node> Nodes       { get; } = new();

  /// <summary>Traces the path of a ray through the scene and returns the color at it's target point.</summary>
  public Color32 CastRay(Point2 screenPos)
  {
    return Background;
  }

  public Ray ProjectRay(Point2 position)
  {
    throw new NotImplementedException();
  }
}

/// <summary>An object in the scene.</summary>
public abstract record Node
{
  /// <summary>Determines if the given object intersects the given <see cref="Ray"/>.</summary>
  public abstract Optional<float> Intersects(Ray ray);
}

/// <summary>A sphere in the scene.</summary>
public sealed record Sphere(Vector3 Position, float Radius, Material Material) : Node
{
  public override Optional<float> Intersects(Ray ray)
  {
    var line = Position - ray.Origin;
    var adjacent = Vector3.Dot(line, ray.Direction);
    var distance2 = Vector3.Dot(line, line) - adjacent * adjacent;
    var radius2 = Radius * Radius;

    if (distance2 > radius2)
    {
      return default;
    }

    var thc = MathF.Sqrt(radius2 - distance2);
    var t0 = adjacent - thc;
    var t1 = adjacent + thc;

    if (t0 < 0.0 && t1 < 0.0)
    {
      return default;
    }

    return t0 < t1 ? t0 : t1;
  }
}

/// <summary>A material that can be sampled for color information.</summary>
public abstract record Material
{
  public abstract Color32 SampleAt(Vector2 uv);

  public sealed record Colored(Color32 Color) : Material
  {
    public override Color32 SampleAt(Vector2 uv)
    {
      return Color;
    }
  }

  public sealed record Textured(Image Image) : Material
  {
    public override Color32 SampleAt(Vector2 uv)
    {
      var positionX = (int)MathF.Floor(uv.X * Image.Width);
      var positionY = (int)MathF.Floor(uv.Y * Image.Height);

      return Image.Pixels[positionX, positionY];
    }
  }
}
