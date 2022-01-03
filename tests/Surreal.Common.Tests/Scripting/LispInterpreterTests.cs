using NUnit.Framework;

namespace Surreal.Scripting;

public class LispInterpreterTests
{
  [Test]
  public void it_should_parse_and_execute_a_simple_expression()
  {
    var interpreter = new LispInterpreter();
    var result      = interpreter.Execute("(+ 1 (+ 1 2))");

    Assert.AreEqual(4f, result);
  }
}
