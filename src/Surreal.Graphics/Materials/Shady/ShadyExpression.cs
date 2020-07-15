namespace Surreal.Graphics.Materials.Shady {
  internal abstract class ShadyExpression {
    public abstract T Accept<T>(Visitor<T> visitor);

    public abstract class Visitor<T> {
      public virtual T Visit(Literal expression) => default!;
      public virtual T Visit(Unary expression)   => default!;
      public virtual T Visit(Binary expression)  => default!;
    }

    public enum UnaryOperation {
      Not
    }

    public enum BinaryOperation {
      Plus,
      Minus,
      Times,
      Divide
    }

    public sealed class Literal : ShadyExpression {
      public string Name  { get; }
      public object Value { get; }

      public Literal(string name, object value) {
        Name  = name;
        Value = value;
      }

      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class Unary : ShadyExpression {
      public UnaryOperation  Operation  { get; }
      public ShadyExpression Expression { get; }

      public Unary(UnaryOperation operation, ShadyExpression expression) {
        Operation  = operation;
        Expression = expression;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public sealed class Binary : ShadyExpression {
      public BinaryOperation Operation { get; }
      public ShadyExpression Left      { get; }
      public ShadyExpression Right     { get; }

      public Binary(BinaryOperation operation, ShadyExpression left, ShadyExpression right) {
        Operation = operation;
        Left      = left;
        Right     = right;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}