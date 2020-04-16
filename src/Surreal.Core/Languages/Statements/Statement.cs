using Surreal.Languages.Visitors;

namespace Surreal.Languages.Statements
{
  public abstract class Statement
  {
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
  }
}
