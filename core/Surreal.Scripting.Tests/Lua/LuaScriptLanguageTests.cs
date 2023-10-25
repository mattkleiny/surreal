using Surreal.Scripting.Lua;

namespace Surreal.Scripting.Tests.Lua;

public class LuaScriptLanguageTests
{
  [Test]
  public async Task it_should_execute_a_simple_script()
  {
    var language = new LuaLanguage();
    var script = await language.LoadAsync("Assets/External/Scripts/HelloWorld.lua");

    var result = script.ExecuteFunction("say_hello", "World");

    result.AsString().Should().Be("Hello, World!");
  }

  [Test]
  public async Task it_should_execute_a_more_complex_script()
  {
    var language = new LuaLanguage();
    var script = await language.LoadAsync("Assets/External/Scripts/HelloGame.lua");
    var game = new TestGameContext();

    script.SetGlobal("Game", Variant.From(game));

    script.ExecuteFunction("tick", 0.16f);

    game.Messages.Dequeue().Should().Be("Hello, World!");
  }

  [UsedByLua]
  private sealed class TestGameContext
  {
    public Queue<string> Messages { get; } = new();

    public void RaiseEvent(string message)
    {
      Messages.Enqueue(message);
    }
  }
}
