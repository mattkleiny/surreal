using Surreal.Languages.Expressions;
using Surreal.Languages.Statements;

namespace Surreal.Languages.Visitors
{
  public abstract class RecursiveVisitor<T> : IStatementVisitor<T>, IExpressionVisitor<T>
  {
    public virtual T Visit(ExpressionStatement statement)
    {
      statement.Expression.Accept(this);

      return default;
    }

    public virtual T Visit(BlockStatement statement)
    {
      foreach (var body in statement.Statements)
      {
        body.Accept(this);
      }

      return default;
    }

    public virtual T Visit(IfStatement statement)
    {
      statement.Condition.Accept(this);
      statement.IfBranch.Accept(this);
      statement.ElseBranch?.Accept(this);

      return default;
    }

    public virtual T Visit(UnaryExpression expression)
    {
      expression.Expression.Accept(this);

      return default;
    }

    public virtual T Visit(BinaryExpression expression)
    {
      expression.Left.Accept(this);
      expression.Right.Accept(this);

      return default;
    }

    public virtual T Visit(VariableExpression expression)
    {
      return default;
    }

    public virtual T Visit(LiteralExpression expression)
    {
      return default;
    }

    public virtual T Visit(CallExpression expression)
    {
      foreach (var parameter in expression.Parameters)
      {
        parameter.Accept(this);
      }

      return default;
    }

    public virtual T Visit(GroupingExpression expression)
    {
      expression.Expression.Accept(this);

      return default;
    }
  }
}
