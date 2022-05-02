namespace RayTracer.Scenes;

public record struct Ray(Vector3 Position, Vector3 Direction)
{
  public static Ray Reflect() => throw new NotImplementedException();
  public static Ray Refract() => throw new NotImplementedException();
}

/// <summary>A scene that can be ray-traced</summary>
public sealed class Scene
{
  public Color32      Background { get; set; } = Color32.Black;
  public List<Object> Objects    { get; }      = new();

  /// <summary>Traces the path of a ray through the scene and returns the color at it's target point.</summary>
  public Color32 CastRay(Point2 screenPos)
  {
    return Background;
  }
}

/// <summary>An object in the scene.</summary>
public abstract record Object;

/// <summary>A sphere in the scene.</summary>
public sealed record Sphere(Vector3 Position, float Radius, Material Material) : Object;

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
