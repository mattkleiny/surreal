using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions {
  [DebuggerDisplay("Variable {Name}")]
  public sealed class VariableExpression : Expression {
    public string Name { get; }

    public VariableExpression(string name) {
      Name = name;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor) {
      return visitor.Visit(this);
    }
  }
}