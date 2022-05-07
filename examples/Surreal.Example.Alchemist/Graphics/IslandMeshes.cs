namespace Alchemist.Graphics;

public static class IslandMeshes
{
  public static Mesh<Vertex2> Create(IGraphicsServer graphics)
  {
    var mesh = new Mesh<Vertex2>(graphics);
    var tessellator = mesh.CreateTessellator();

    tessellator.AddQuad(
      new Vertex2(new(-0.5f, -0.5f), Color.Red, new Vector2(0f, 0f)),
      new Vertex2(new(-0.5f, 0.5f), Color.Green, new Vector2(0f, 1f)),
      new Vertex2(new(0.5f, 0.5f), Color.Blue, new Vector2(1f, 1f)),
      new Vertex2(new(0.5f, -0.5f), Color.White, new Vector2(1f, 0f))
    );

    tessellator.WriteTo(mesh);

    return mesh;
  }
}
