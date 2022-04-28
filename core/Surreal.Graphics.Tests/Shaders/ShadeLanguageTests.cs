namespace Surreal.Graphics.Shaders;

public class ShadeLanguageTests
{
  [Test]
  [TestCase("2 + 2")]
  public void it_should_parse_simple_expressions(string expression)
  {
    ShadeLanguage.Parse(expression);
  }
}
