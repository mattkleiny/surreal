using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Languages;
using static Surreal.Diagnostics.Console.Interpreter.ConsoleExpression;

namespace Surreal.Diagnostics.Console.Interpreter {
  public sealed class ConsoleInterpreter : IConsoleInterpreter {
    private readonly BindingCollection bindings = new BindingCollection();

    private readonly ExecutionVisitor visitor;

    public ConsoleInterpreter(Action<IConsoleInterpreterBindings> builder) {
      builder(bindings);

      visitor = new ExecutionVisitor(bindings);
    }

    public string? Evaluate(string raw) {
      try {
        var text   = SourceText.FromString(raw);
        var parser = new ConsoleLanguageParser(text);
        var result = parser.Expression().Accept(visitor);

        return result?.ToString() ?? "OK";
      } catch (Exception exception) {
        return exception.Message;
      }
    }

    private sealed class ExecutionVisitor : Visitor<object?> {
      private readonly BindingCollection bindings;

      public ExecutionVisitor(BindingCollection bindings) {
        this.bindings = bindings;
      }

#pragma warning disable 8620
      public override object? Visit(Call expression) {
        var symbol     = expression.Symbol.ToString();
        var parameters = expression.Parameters.Select(_ => _.Accept(this)).ToArray();

        if (bindings.Lookup(symbol, out var handler)) {
          return handler.Invoke(parameters);
        }

        throw new Exception($"An unrecognized function was encountered: {symbol}");
      }
#pragma warning restore 8620

      public override object? Visit(Unary expression) => expression.Operation switch {
          UnaryOperation.Not    => !IsTruthy(expression.Expression),
          UnaryOperation.Negate => -ToNumber(expression.Expression),
          _                     => throw new NotSupportedException($"The operator '{expression.Operation}' is not supported"),
      };

      public override object? Visit(Binary expression) => expression.Operation switch {
          BinaryOperation.Plus   => ToNumber(expression.Left) + ToNumber(expression.Right),
          BinaryOperation.Minus  => ToNumber(expression.Left) - ToNumber(expression.Right),
          BinaryOperation.Times  => ToNumber(expression.Left) * ToNumber(expression.Right),
          BinaryOperation.Divide => ToNumber(expression.Left) / ToNumber(expression.Right),
          _                      => throw new NotSupportedException($"The operator '{expression.Operation}' is not supported"),
      };

      public override object? Visit(Literal expression) => expression.Value;

      private bool  IsTruthy(ConsoleExpression expression) => To<bool>(expression);
      private float ToNumber(ConsoleExpression expression) => To<float>(expression);

      private T To<T>(ConsoleExpression expression) {
        var value = expression.Accept(this);
        if (value is T type) return type;

        throw new Exception($"The result of the expression is not a {typeof(T)} type!");
      }
    }

    private sealed class BindingCollection : IConsoleInterpreterBindings {
      private readonly IDictionary<string, Binding> actionsByName =
          new Dictionary<string, Binding>(StringComparer.OrdinalIgnoreCase);

      public bool Lookup(string name, out Binding action) {
        return actionsByName.TryGetValue(name, out action);
      }

      public void Add(string name, Binding binding) {
        actionsByName.Add(name, parameters => {
          var result = binding(parameters);

          if (result != null) {
            return result.ToString();
          }

          return string.Empty;
        });
      }
    }
  }
}