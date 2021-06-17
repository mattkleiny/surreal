using System.Collections.Generic;
using System.Diagnostics;
using Surreal.Collections;
using Surreal.Diagnostics.Console.Interpreter;

namespace Surreal.Diagnostics.Console {
  public sealed class GameConsole : IGameConsole {
    private readonly IConsoleInterpreter interpreter;
    private readonly RingBuffer<string>  history;

    public GameConsole(IConsoleInterpreter interpreter, int capacity = 1000) {
      Debug.Assert(capacity > 0, "capacity > 0");

      this.interpreter = interpreter;

      history = new RingBuffer<string>(capacity);
    }

    public IEnumerable<string> History => history;

    public void WriteLine(string element) {
      history.Add(element);
    }

    public void Evaluate(string expression) {
      WriteLine(interpreter.Evaluate(expression));
    }

    public void Clear() {
      history.Clear();
    }
  }
}