﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Languages.Expressions;
using Surreal.Languages.Visitors;

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
        var parser    = new ConsoleLanguageParser(raw);
        var statement = parser.Statement();
        var result    = statement.Accept(visitor);

        return result?.ToString() ?? "OK";
      }
      catch (Exception exception) {
        return exception.Message;
      }
    }

    private sealed class ExecutionVisitor : RecursiveVisitor<object?> {
      private readonly BindingCollection bindings;

      public ExecutionVisitor(BindingCollection bindings) {
        this.bindings = bindings;
      }

#pragma warning disable 8620
      public override object? Visit(CallExpression expression) {
        var symbol     = expression.Symbol.ToString();
        var parameters = expression.Parameters.Select(_ => _.Accept(this)).ToArray();

        if (bindings.Lookup(symbol, out var handler)) {
          return handler.Invoke(parameters);
        }

        throw new Exception($"An unrecognized function was encountered: {symbol}");
      }
#pragma warning restore 8620

      public override object? Visit(UnaryExpression expression) => expression.Operation switch {
          UnaryOperation.Not    => !IsTruthy(expression.Expression),
          UnaryOperation.Negate => -ToNumber(expression.Expression),
          _                     => throw new NotSupportedException($"The operator '{expression.Operation}' is not supported"),
      };

      public override object? Visit(BinaryExpression expression) => expression.Operation switch {
          BinaryOperation.Plus   => ToNumber(expression.Left) + ToNumber(expression.Right),
          BinaryOperation.Minus  => ToNumber(expression.Left) - ToNumber(expression.Right),
          BinaryOperation.Times  => ToNumber(expression.Left) * ToNumber(expression.Right),
          BinaryOperation.Divide => ToNumber(expression.Left) / ToNumber(expression.Right),
          _                      => throw new NotSupportedException($"The operator '{expression.Operation}' is not supported"),
      };

      public override object? Visit(LiteralExpression expression) => expression.Value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private bool IsTruthy(Expression expression) => To<bool>(expression);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private float ToNumber(Expression expression) => To<float>(expression);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private T To<T>(Expression expression) {
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