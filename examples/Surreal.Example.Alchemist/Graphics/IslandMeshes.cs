namespace Alchemist.Graphics;

public static class IslandMeshes
{
  public static Mesh<Vertex2> Create(IGraphicsServer graphics, int numberOfPoints = 8, Seed seed = default)
  {
    return Mesh.Build<Vertex2>(graphics, tessellator =>
    {
      const float innerRadius = 0.5f;
      const float outerRadius = 1.25f;

      var random = seed.ToRandom();
      var points = new SpanList<Vertex2>(stackalloc Vertex2[numberOfPoints]);

      var theta = 0f;

      for (var i = 0; i < numberOfPoints; i++)
      {
        theta += 2 * MathF.PI / numberOfPoints;

        var radius = random.NextFloat(innerRadius, outerRadius);

        var x = radius * MathF.Cos(theta);
        var y = radius * MathF.Sin(theta);

        points.Add(new Vertex2(new(x, y), Color.White, new Vector2(0f, 0f)));
      }

      tessellator.AddTriangleFan(points);
    });
  }
}
