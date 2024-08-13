using Jint;

namespace Surreal.Scripting.Javascript;

/// <summary>
/// A runtime for executing Javascript code.
/// </summary>
public sealed class JavascriptRuntime : IDisposable
{
  private readonly Engine _engine = new();

  public Variant Execute(string code)
  {
    return _engine.Evaluate(code).ToVariant();
  }

  public void Dispose()
  {
    _engine.Dispose();
  }
}
