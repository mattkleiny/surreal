using System.Collections.Generic;

namespace Surreal.Diagnostics.Console
{
  public interface IGameConsole
  {
    IEnumerable<string> History { get; }

    void WriteLine(string element);
    void Evaluate(string expression);
    void Clear();
  }
}
