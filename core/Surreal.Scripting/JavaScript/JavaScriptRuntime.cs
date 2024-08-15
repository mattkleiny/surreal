using Jint;

namespace Surreal.Scripting.JavaScript;

/// <summary>
/// A runtime for executing Javascript code.
/// </summary>
public sealed class JavaScriptRuntime : IDisposable
{
  private readonly Engine _engine = new();

  public Variant Evaluate(string code)
  {
    return _engine.Evaluate(code).ToVariant();
  }

  public void Dispose()
  {
    _engine.Dispose();
  }
}
