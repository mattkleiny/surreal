using Surreal.Languages.Expressions;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Statements
{
  public sealed class IfStatement : Statement
  {
    public Expression Condition  { get; }
    public Statement  IfBranch   { get; }
    public Statement? ElseBranch { get; }

    public IfStatement(Expression condition, Statement ifBranch, Statement elseBranch)
    {
      Condition  = condition;
      IfBranch   = ifBranch;
      ElseBranch = elseBranch;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
