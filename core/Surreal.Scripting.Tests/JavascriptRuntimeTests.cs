using Surreal.Scripting.Javascript;

namespace Surreal.Scripting;

public class JavascriptRuntimeTests
{
  [Test]
  public void it_should_execute_simple_instructions()
  {
    using var runtime = new JavascriptRuntime();

    var result = runtime.Execute("return 1 + 2;");

    Assert.AreEqual(3, result.AsDouble());
  }
}
