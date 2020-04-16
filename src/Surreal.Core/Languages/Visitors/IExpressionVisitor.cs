using Surreal.Languages.Expressions;

namespace Surreal.Languages.Visitors
{
  public interface IExpressionVisitor<out T>
  {
    T Visit(UnaryExpression expression);
    T Visit(BinaryExpression expression);
    T Visit(VariableExpression expression);
    T Visit(LiteralExpression expression);
    T Visit(CallExpression expression);
    T Visit(GroupingExpression expression);
  }
}
