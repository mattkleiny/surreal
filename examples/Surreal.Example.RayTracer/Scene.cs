namespace Surreal;

/// <summary>A scene that can be ray-traced</summary>
public sealed class Scene
{
  // scene properties
  public int Width { get; set; } = 1920;
  public int Height { get; set; } = 1080;
  public Color Background { get; set; } = Color.Black;
  public Angle FieldOfView { get; set; } = Angle.FromDegrees(90f);

  // scene contents
  public List<Node> Nodes { get; } = new();
  public List<Light> Lights { get; } = new();

  /// <summary>Traces the path of a ray through the scene and returns the color at it's target point.</summary>
  public Color Sample(Point2 screenPos)
  {
    return Trace(ProjectRay(screenPos));
  }

  private Color Trace(Ray ray, int depth = 0, int maxDepth = 16)
  {
    if (depth < maxDepth)
    {
      var intersection = FindIntersectingObject(ray);
      if (intersection is { IsSome: true, Value: var (node, distance) })
      {
        var material = node.Material;

        var hitPoint = ray.Origin + ray.Direction * distance;
        var surfaceNormal = node.CalculateNormal(hitPoint);
        var surfaceUV = node.CalculateUV(hitPoint);

        var color = ApplyDiffuseShading(material, hitPoint, surfaceNormal, surfaceUV);

        var reflectivity = material.Reflectivity;
        if (reflectivity > 0f)
        {
          var reflectionRay = Ray.Reflect(surfaceNormal, ray.Direction, hitPoint, float.Epsilon);

          color *= 1.0f - reflectivity;
          color += Trace(reflectionRay, depth + 1, maxDepth) * reflectivity;
        }

        return color;
      }
    }

    return Background;
  }

  private Color ApplyDiffuseShading(Material material, Vector3 hitPoint, Vector3 surfaceNormal, Vector2 surfaceUV)
  {
    var color = Color.Black;
    var albedo = material.SampleAt(surfaceUV);

    foreach (var light in Lights)
    {
      var directionToLight = -light.Direction;

      var shadowRay = new Ray(hitPoint + surfaceNormal * float.Epsilon, directionToLight);
      var isInShadow = FindIntersectingObject(shadowRay).IsSome;

      var lightPower = Vector3.Dot(surfaceNormal, directionToLight) * (isInShadow ? 0f : light.Intensity);
      var lightReflected = albedo / MathF.PI;
      var lightColor = light.Emissive * lightPower * lightReflected;

      color += albedo * lightColor;
    }

    return color;
  }

  private Optional<(Node Node, float Distance)> FindIntersectingObject(Ray ray)
  {
    var bestDistance = float.MaxValue;
    var bestNode = default(Node?);

    foreach (var node in Nodes)
    {
      var intersection = node.Intersects(ray);
      if (intersection is { IsSome: true, Value: var distance })
      {
        if (distance < bestDistance)
        {
          bestDistance = distance;
          bestNode = node;
        }
      }
    }

    if (bestNode != null)
    {
      return (bestNode, bestDistance);
    }

    return default;
  }

  private Ray ProjectRay(Point2 position)
  {
    var fovAdjustment = MathF.Tan(FieldOfView.Radians / 2);
    var aspectRatio = Width / Height;

    var sensorX = ((position.X + 0.5f) / Width * 2.0f - 1.0f) * aspectRatio * fovAdjustment;
    var sensorY = (1.0f - (position.Y + 0.5f) / Height * 2.0f) * fovAdjustment;

    var direction = Vector3.Normalize(new Vector3(sensorX, sensorY, -1.0f));

    return new Ray(Vector3.Zero, direction);
  }
}

/// <summary>A ray in 3-space.</summary>
public record struct Ray(Vector3 Origin, Vector3 Direction)
{
  public static Ray Reflect(Vector3 normal, Vector3 incidence, Vector3 intersection, float bias)
  {
    var origin = intersection + normal * bias;
    var direction = incidence - normal * 2.0f * Vector3.Dot(incidence, normal);

    return new Ray(origin, direction);
  }
}

/// <summary>A light in a ray tracing scene.</summary>
public sealed record Light(Vector3 Direction, Color Emissive, float Intensity);

/// <summary>An object in the scene.</summary>
public abstract record Node(Material Material)
{
  public abstract Optional<float> Intersects(Ray ray);

  public abstract Vector3 CalculateNormal(Vector3 point);
  public abstract Vector2 CalculateUV(Vector3 point);
}

/// <summary>A sphere in the scene.</summary>
public sealed record Sphere(Vector3 Center, float Radius, Material Material) : Node(Material)
{
  public override Optional<float> Intersects(Ray ray)
  {
    var line = Center - ray.Origin;
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

  public override Vector3 CalculateNormal(Vector3 point)
  {
    return Vector3.Normalize(point - Center);
  }

  public override Vector2 CalculateUV(Vector3 point)
  {
    var spherical = point - Center;

    var u = (1.0f + MathF.Atan(spherical.X) / MathF.PI) * 0.5f;
    var v = MathF.Acos(spherical.Y / Radius) / MathF.PI;

    return new Vector2(u, v);
  }
}

/// <summary>A plane in the scene.</summary>
public sealed record Plane(Vector3 Origin, Vector3 Normal, Material Material) : Node(Material)
{
  public override Optional<float> Intersects(Ray ray)
  {
    var d = Vector3.Dot(Normal, ray.Direction);
    if (d >= float.Epsilon)
    {
      var direction = Origin - ray.Origin;
      var distance = Vector3.Dot(direction, Normal) / d;

      if (distance >= 0f)
      {
        return distance;
      }
    }

    return default;
  }

  public override Vector3 CalculateNormal(Vector3 point)
  {
    return -Normal;
  }

  public override Vector2 CalculateUV(Vector3 point)
  {
    return new Vector2(0f, 0f); // TODO: implement me
  }
}

/// <summary>A material that can be sampled for color information.</summary>
public abstract record Material(float Reflectivity)
{
  public abstract Color SampleAt(Vector2 uv);

  /// <summary>A simple solid color <see cref="Material" />.</summary>
  public sealed record Solid(Color Color, float Reflectivity) : Material(Reflectivity)
  {
    public override Color SampleAt(Vector2 uv)
    {
      return Color;
    }
  }

  /// <summary>A simple image-based <see cref="Material" />.</summary>
  public sealed record Textured(Image Image, float Reflectivity) : Material(Reflectivity)
  {
    public override Color SampleAt(Vector2 uv)
    {
      var positionX = (int) MathF.Floor(uv.X * Image.Width) % Image.Width;
      var positionY = (int) MathF.Floor(uv.Y * Image.Height) % Image.Height;

      return Image.Pixels[positionX, positionY];
    }
  }
}


