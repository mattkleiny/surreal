using Surreal.Languages.Expressions;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Statements {
  public sealed class ExpressionStatement : Statement {
    public Expression Expression { get; }

    public ExpressionStatement(Expression expression) {
      Expression = expression;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor) {
      return visitor.Visit(this);
    }
  }
}