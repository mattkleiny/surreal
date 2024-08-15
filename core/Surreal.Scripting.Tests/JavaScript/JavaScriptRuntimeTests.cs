namespace Surreal.Scripting.JavaScript;

public class JavaScriptRuntimeTests
{
  [Test]
  public void it_should_execute_simple_instructions()
  {
    using var runtime = new JavaScriptRuntime();

    var result = runtime.Evaluate("return 1 + 2;");

    Assert.AreEqual(3, result.AsDouble());
  }
}
