using Surreal.IO;
using Surreal.Scripting.Lua;

namespace Surreal.Lua;

public class LuaScriptLanguageTests
{
  [Test]
  public void it_should_execute_code_with_side_effect()
  {
    using var language = new LuaLanguage();

    var result = language.ExecuteCode("print(\"Hello, world!\")");

    Assert.AreEqual(Variant.Null, result);
  }

  [Test]
  public void it_should_execute_code_and_return()
  {
    using var language = new LuaLanguage();

    var result = language.ExecuteCode("return 2 + 2");

    result.AsInt().Should().Be(4);
  }

  [Test]
  [TestCase("local://Assets/External/scripts/HelloWorld.lua")]
  public void it_should_execute_a_file(VirtualPath path)
  {
    using var language = new LuaLanguage();

    var result = language.ExecuteFile(path);

    result.AsInt().Should().Be(42);
  }

  [Test]
  public void it_should_execute_a_file_with_context()
  {
    using var language = new LuaLanguage();
    var context = new GameContext();

    language["Game"] = context;

    language.ExecuteFile("local://Assets/External/scripts/HelloGame.lua");

    if (!language.TryGetCallable("tick", out var tick))
    {
      Assert.Fail("Could not get tick function from Lua.");
    }

    tick.Invoke(1f / 60f);

    context.Events.Should().Contain("Hello, world!");
  }

  [Test]
  public void it_should_pass_global_state()
  {
    using var language = new LuaLanguage();

    language["test"] = 42;

    var result = language.ExecuteCode("return test");

    result.AsInt().Should().Be(42);
  }

  [Test]
  public void it_should_yield_functions_from_lua()
  {
    using var language = new LuaLanguage();

    language.ExecuteCode("function test() return 42 end");

    if (!language.TryGetCallable("test", out var callable))
    {
      Assert.Fail("Could not get function from Lua.");
    }

    callable.Invoke().AsInt().Should().Be(42);
  }

  private sealed class GameContext
  {
    public Queue<string> Events { get; } = new();

    [UsedByLua]
    public void RaiseEvent(string message)
    {
      Events.Enqueue(message);
    }
  }
}
