using System.Diagnostics;

namespace Surreal.Diagnostics.Console.Interpreter {
  public abstract class ConsoleExpression {
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<out T> {
      T Visit(UnaryExpression expression);
      T Visit(BinaryExpression expression);
      T Visit(VariableExpression expression);
      T Visit(LiteralExpression expression);
      T Visit(CallExpression expression);
    }

    public abstract class RecursiveVisitor<T> : IVisitor<T> {
      public virtual T Visit(UnaryExpression expression) {
        expression.Expression.Accept(this);

        return default!;
      }

      public virtual T Visit(BinaryExpression expression) {
        expression.Left.Accept(this);
        expression.Right.Accept(this);

        return default!;
      }

      public virtual T Visit(VariableExpression expression) {
        return default!;
      }

      public virtual T Visit(LiteralExpression expression) {
        return default!;
      }

      public virtual T Visit(CallExpression expression) {
        foreach (var parameter in expression.Parameters) {
          parameter.Accept(this);
        }

        return default!;
      }
    }

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

    [DebuggerDisplay("{Value}")]
    public sealed class LiteralExpression : ConsoleExpression {
      public object Value { get; }

      public LiteralExpression(object value) {
        Value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    [DebuggerDisplay("{Operation} {Expression}")]
    public sealed class UnaryExpression : ConsoleExpression {
      public UnaryOperation    Operation  { get; }
      public ConsoleExpression Expression { get; }

      public UnaryExpression(UnaryOperation operation, ConsoleExpression expression) {
        Operation  = operation;
        Expression = expression;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    [DebuggerDisplay("{Left} {Operation} {Right}")]
    public sealed class BinaryExpression : ConsoleExpression {
      public BinaryOperation   Operation { get; }
      public ConsoleExpression Left      { get; }
      public ConsoleExpression Right     { get; }

      public BinaryExpression(BinaryOperation operation, ConsoleExpression left, ConsoleExpression right) {
        Operation = operation;
        Left      = left;
        Right     = right;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    [DebuggerDisplay("Call {Symbol}")]
    public sealed class CallExpression : ConsoleExpression {
      public object              Symbol     { get; }
      public ConsoleExpression[] Parameters { get; }

      public CallExpression(object symbol, params ConsoleExpression[] parameters) {
        Symbol     = symbol;
        Parameters = parameters;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    [DebuggerDisplay("Variable {Name}")]
    public sealed class VariableExpression : ConsoleExpression {
      public string Name { get; }

      public VariableExpression(string name) {
        Name = name;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }
  }
}