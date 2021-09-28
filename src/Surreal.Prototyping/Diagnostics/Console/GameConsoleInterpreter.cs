using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Surreal.Diagnostics.Console.GameConsoleExpression;

namespace Surreal.Diagnostics.Console
{
  public delegate object? GameConsoleBinding(params object[] arguments);

  /// <summary>An interpreter for expressions in the game console.</summary>
  public interface IGameConsoleInterpreter
  {
    string Evaluate(string expression);
  }

  /// <summary>Provides bindings from method names to functions in the application.</summary>
  public interface IGameConsoleBindings
  {
    void Add(string name, GameConsoleBinding binding);

    void Add(string name, Action action)
    {
      Add(name, _ =>
      {
        action.Invoke();
        return string.Empty;
      });
    }
  }

  /// <summary>The default <see cref="IGameConsoleInterpreter"/>.</summary>
  public sealed class GameConsoleInterpreter : IGameConsoleInterpreter
  {
    private readonly BindingCollection bindings = new();
    private readonly ExecutionVisitor  visitor;

    public GameConsoleInterpreter(Action<IGameConsoleBindings> builder)
    {
      builder(bindings);

      visitor = new ExecutionVisitor(bindings);
    }

    public string Evaluate(string raw)
    {
      try
      {
        using var reader = new StringReader(raw);

        var parser = GameConsoleParser.ParseAsync(reader).Result;
        var result = parser.Expression().Accept(visitor);

        return result?.ToString() ?? "OK";
      }
      catch (Exception exception)
      {
        return exception.Message;
      }
    }

    private sealed class ExecutionVisitor : RecursiveVisitor<object?>
    {
      private readonly BindingCollection bindings;

      public ExecutionVisitor(BindingCollection bindings)
      {
        this.bindings = bindings;
      }

#pragma warning disable 8620
      public override object? Visit(CallExpression expression)
      {
        var symbol     = expression.Symbol.ToString()!;
        var parameters = expression.Parameters.Select(_ => _.Accept(this)).ToArray();

        if (bindings.Lookup(symbol, out var handler))
        {
          return handler.Invoke(parameters);
        }

        throw new Exception($"An unrecognized function was encountered: {symbol}");
      }
#pragma warning restore 8620

      public override object? Visit(UnaryExpression expression) => expression.Operator switch
      {
        UnaryOperation.Not    => !IsTruthy(expression.Expression),
        UnaryOperation.Negate => -ToNumber(expression.Expression),
        _                     => throw new NotSupportedException($"The operator '{expression.Operator}' is not supported")
      };

      public override object? Visit(BinaryExpression expression) => expression.Operator switch
      {
        BinaryOperation.Plus   => ToNumber(expression.Left) + ToNumber(expression.Right),
        BinaryOperation.Minus  => ToNumber(expression.Left) - ToNumber(expression.Right),
        BinaryOperation.Times  => ToNumber(expression.Left) * ToNumber(expression.Right),
        BinaryOperation.Divide => ToNumber(expression.Left) / ToNumber(expression.Right),
        _                      => throw new NotSupportedException($"The operator '{expression.Operator}' is not supported")
      };

      public override object Visit(LiteralExpression expression) => expression.Value;

      private bool  IsTruthy(GameConsoleExpression expression) => To<bool>(expression);
      private float ToNumber(GameConsoleExpression expression) => To<float>(expression);

      private T To<T>(GameConsoleExpression expression)
      {
        var value = expression.Accept(this);
        if (value is T type) return type;

        throw new Exception($"The result of the expression is not a {typeof(T)} type!");
      }
    }

    private sealed class BindingCollection : IGameConsoleBindings
    {
      private readonly Dictionary<string, GameConsoleBinding> actionsByName = new(StringComparer.OrdinalIgnoreCase);

      public bool Lookup(string name, out GameConsoleBinding action)
      {
        return actionsByName.TryGetValue(name, out action!);
      }

      public void Add(string name, GameConsoleBinding binding)
      {
        actionsByName.Add(name, parameters =>
        {
          var result = binding(parameters);

          if (result != null)
          {
            return result.ToString();
          }

          return string.Empty;
        });
      }
    }
  }
}
