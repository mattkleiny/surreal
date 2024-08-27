namespace Surreal.Scripting.Languages.Lox;

public class LoxScriptLanguageTests
{
  [Test]
  public async Task it_should_parse_a_simple_program()
  {
    var language = new LoxScriptLanguage();
    var reader = new StringReader("print \"Hello, world!\"");

    var module = await language.ParseAsync(reader);

    Assert.NotNull(module);
  }
}
