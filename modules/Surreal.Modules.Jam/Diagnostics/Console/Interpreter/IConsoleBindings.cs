using System;

namespace Surreal.Diagnostics.Console.Interpreter {
  public delegate object? Binding(params object[] arguments);

  public interface IConsoleBindings {
    void Add(string name, Binding binding);

    void Add(string name, Action action) {
      Add(name, _ => {
        action.Invoke();
        return string.Empty;
      });
    }
  }
}