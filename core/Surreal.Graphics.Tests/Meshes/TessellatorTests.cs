namespace Surreal.Graphics.Meshes;

public class TessellatorTests
{
  [Test]
  public void it_should_tessellate_simple_figures()
  {
    var tessellator = new Tessellator<Vertex2>();

    tessellator.AddLine(Vector2.Zero, Vector2.One);
  }

  // TODO: add more tests
}



