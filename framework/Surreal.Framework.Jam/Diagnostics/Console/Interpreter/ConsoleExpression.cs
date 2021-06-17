using System.Diagnostics;

namespace Surreal.Diagnostics.Console.Interpreter {
  public abstract record ConsoleExpression {
    public abstract T? Accept<T>(IVisitor<T> visitor);

    public enum UnaryOperation {
      Not,
      Negate,
    }

    public enum BinaryOperation {
      Plus,
      Minus,
      Times,
      Divide,
    }

    public interface IVisitor<out T> {
      T? Visit(LiteralExpression expression);
      T? Visit(UnaryExpression expression);
      T? Visit(BinaryExpression expression);
      T? Visit(CallExpression expression);
      T? Visit(VariableExpression expression);
    }

    [DebuggerDisplay("{Value}")]
    public sealed record LiteralExpression(object Value) : ConsoleExpression {
      public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
    }

    [DebuggerDisplay("{Operator} {Expression}")]
    public sealed record UnaryExpression(UnaryOperation Operator, ConsoleExpression Expression) : ConsoleExpression {
      public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
    }

    [DebuggerDisplay("{Left} {Operator} {Right}")]
    public sealed record BinaryExpression(BinaryOperation Operator, ConsoleExpression Left, ConsoleExpression Right) : ConsoleExpression {
      public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
    }

    [DebuggerDisplay("Call {Symbol}")]
    public sealed record CallExpression(object Symbol, params ConsoleExpression[] Parameters) : ConsoleExpression {
      public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
    }

    [DebuggerDisplay("Variable {Name}")]
    public sealed record VariableExpression(string Name) : ConsoleExpression {
      public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
    }

    public abstract class RecursiveVisitor<T> : IVisitor<T> {
      public virtual T? Visit(UnaryExpression expression) {
        expression.Expression.Accept(this);

        return default;
      }

      public virtual T? Visit(BinaryExpression expression) {
        expression.Left.Accept(this);
        expression.Right.Accept(this);

        return default;
      }

      public virtual T? Visit(VariableExpression expression) {
        return default;
      }

      public virtual T? Visit(LiteralExpression expression) {
        return default;
      }

      public virtual T? Visit(CallExpression expression) {
        foreach (var parameter in expression.Parameters) {
          parameter.Accept(this);
        }

        return default;
      }
    }
  }
}