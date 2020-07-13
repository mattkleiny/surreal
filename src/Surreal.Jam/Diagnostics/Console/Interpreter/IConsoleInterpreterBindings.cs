using System;

namespace Surreal.Diagnostics.Console.Interpreter {
  public delegate object? Binding(params object[] arguments);

  public interface IConsoleInterpreterBindings {
    void Add(string name, Binding binding);

    void Add(string name, Action action) {
      Add(name, parameters => {
        action.Invoke();
        return string.Empty;
      });
    }
  }
}