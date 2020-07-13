namespace Surreal.Diagnostics.Console.Interpreter {
  public interface IConsoleInterpreter {
    string? Evaluate(string expression);
  }
}