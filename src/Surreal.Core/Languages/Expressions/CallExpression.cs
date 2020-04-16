using System.Diagnostics;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions
{
  [DebuggerDisplay("Call {Symbol}")]
  public sealed class CallExpression : Expression
  {
    public object       Symbol     { get; }
    public Expression[] Parameters { get; }

    public CallExpression(object symbol, params Expression[] parameters)
    {
      Symbol     = symbol;
      Parameters = parameters;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}