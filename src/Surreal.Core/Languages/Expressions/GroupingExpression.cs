using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions
{
  [DebuggerDisplay("({Expression})")]
  public sealed class GroupingExpression : Expression
  {
    public Expression Expression { get; }

    public GroupingExpression(Expression expression)
    {
      Expression = expression;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}