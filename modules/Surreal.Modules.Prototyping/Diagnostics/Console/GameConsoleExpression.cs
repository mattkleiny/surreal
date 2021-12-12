using System.Diagnostics;

namespace Surreal.Diagnostics.Console;

internal abstract record GameConsoleExpression
{
  public abstract T? Accept<T>(IVisitor<T> visitor);

  public enum UnaryOperation
  {
    Not,
    Negate
  }

  public enum BinaryOperation
  {
    Plus,
    Minus,
    Times,
    Divide
  }

  public interface IVisitor<out T>
  {
    T? Visit(LiteralExpression expression);
    T? Visit(UnaryExpression expression);
    T? Visit(BinaryExpression expression);
    T? Visit(CallExpression expression);
    T? Visit(VariableExpression expression);
  }

  [DebuggerDisplay("{Value}")]
  public sealed record LiteralExpression(object Value) : GameConsoleExpression
  {
    public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
  }

  [DebuggerDisplay("{Operator} {Expression}")]
  public sealed record UnaryExpression(UnaryOperation Operator, GameConsoleExpression Expression) : GameConsoleExpression
  {
    public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
  }

  [DebuggerDisplay("{Left} {Operator} {Right}")]
  public sealed record BinaryExpression(BinaryOperation Operator, GameConsoleExpression Left, GameConsoleExpression Right) : GameConsoleExpression
  {
    public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
  }

  [DebuggerDisplay("Call {Symbol}")]
  public sealed record CallExpression(object Symbol, params GameConsoleExpression[] Parameters) : GameConsoleExpression
  {
    public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
  }

  [DebuggerDisplay("Variable {Name}")]
  public sealed record VariableExpression(string Name) : GameConsoleExpression
  {
    public override T? Accept<T>(IVisitor<T> visitor) where T : default => visitor.Visit(this);
  }

  public abstract class RecursiveVisitor<T> : IVisitor<T>
  {
    public virtual T? Visit(UnaryExpression expression)
    {
      expression.Expression.Accept(this);

      return default;
    }

    public virtual T? Visit(BinaryExpression expression)
    {
      expression.Left.Accept(this);
      expression.Right.Accept(this);

      return default;
    }

    public virtual T? Visit(VariableExpression expression)
    {
      return default;
    }

    public virtual T? Visit(LiteralExpression expression)
    {
      return default;
    }

    public virtual T? Visit(CallExpression expression)
    {
      foreach (var parameter in expression.Parameters)
      {
        parameter.Accept(this);
      }

      return default;
    }
  }
}