using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions {
  [DebuggerDisplay("{Operation} {Expression}")]
  public sealed class UnaryExpression : Expression {
    public UnaryOperation Operation  { get; }
    public Expression     Expression { get; }

    public UnaryExpression(UnaryOperation operation, Expression expression) {
      Operation  = operation;
      Expression = expression;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor) {
      return visitor.Visit(this);
    }
  }
}