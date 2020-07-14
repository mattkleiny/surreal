using System.Diagnostics;

namespace Surreal.Diagnostics.Console.Interpreter {
  public abstract class ConsoleExpression {
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<out T> {
      T Visit(Unary expression);
      T Visit(Binary expression);
      T Visit(VariableExpression expression);
      T Visit(Literal expression);
      T Visit(Call expression);
    }

    public abstract class RecursiveVisitor<T> : IVisitor<T> {
      public virtual T Visit(Unary expression) {
        expression.Expression.Accept(this);

        return default!;
      }

      public virtual T Visit(Binary expression) {
        expression.Left.Accept(this);
        expression.Right.Accept(this);

        return default!;
      }

      public virtual T Visit(VariableExpression expression) {
        return default!;
      }

      public virtual T Visit(Literal expression) {
        return default!;
      }

      public virtual T Visit(Call expression) {
        foreach (var parameter in expression.Parameters) {
          parameter.Accept(this);
        }

        return default!;
      }
    }

    public enum BinaryOperation {
      Plus,
      Minus,
      Times,
      Divide,
    }

    public enum UnaryOperation {
      Not,
      Negate,
    }

    [DebuggerDisplay("{Operation} {Expression}")]
    public sealed class Unary : ConsoleExpression {
      public UnaryOperation    Operation  { get; }
      public ConsoleExpression Expression { get; }

      public Unary(UnaryOperation operation, ConsoleExpression expression) {
        Operation  = operation;
        Expression = expression;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    [DebuggerDisplay("{Left} {Operation} {Right}")]
    public sealed class Binary : ConsoleExpression {
      public BinaryOperation   Operation { get; }
      public ConsoleExpression Left      { get; }
      public ConsoleExpression Right     { get; }

      public Binary(BinaryOperation operation, ConsoleExpression left, ConsoleExpression right) {
        Operation = operation;
        Left      = left;
        Right     = right;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    [DebuggerDisplay("{Value}")]
    public sealed class Literal : ConsoleExpression {
      public object Value { get; }

      public Literal(object value) {
        Value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    [DebuggerDisplay("Call {Symbol}")]
    public sealed class Call : ConsoleExpression {
      public object              Symbol     { get; }
      public ConsoleExpression[] Parameters { get; }

      public Call(object symbol, params ConsoleExpression[] parameters) {
        Symbol     = symbol;
        Parameters = parameters;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    [DebuggerDisplay("Variable {Name}")]
    public sealed class VariableExpression : ConsoleExpression {
      public string Name { get; }

      public VariableExpression(string name) {
        Name = name;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}