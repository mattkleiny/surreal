using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions {
  [DebuggerDisplay("{Left} {Operation} {Right}")]
  public sealed class BinaryExpression : Expression {
    public BinaryOperation Operation { get; }
    public Expression      Left      { get; }
    public Expression      Right     { get; }

    public BinaryExpression(BinaryOperation operation, Expression left, Expression right) {
      Operation = operation;
      Left      = left;
      Right     = right;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor) {
      return visitor.Visit(this);
    }
  }
}