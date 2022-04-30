using Surreal.Graphics.UI.Editors;

namespace Surreal.Graphics.UI;

public class ImmediateModeTests
{
  [Test]
  public void it_should_work()
  {
    using var canvas = new ImmediateModeCanvas(new HeadlessGraphicsServer());

    var position = Vector2.Zero;

    canvas.DrawEditor(ref position);
    canvas.DrawText($"This is a test {position}");
  }
}
