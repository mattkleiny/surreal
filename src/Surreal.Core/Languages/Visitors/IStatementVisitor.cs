using Surreal.Languages.Statements;

namespace Surreal.Languages.Visitors
{
  public interface IStatementVisitor<out T>
  {
    T Visit(ExpressionStatement statement);
    T Visit(BlockStatement statement);
    T Visit(IfStatement statement);
  }
}
