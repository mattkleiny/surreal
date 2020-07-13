using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions {
  [DebuggerDisplay("{Value}")]
  public sealed class LiteralExpression : Expression {
    public object Value { get; }

    public LiteralExpression(object value) {
      Value = value;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor) {
      return visitor.Visit(this);
    }
  }
}