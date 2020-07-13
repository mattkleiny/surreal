using Surreal.Languages.Visitors;

namespace Surreal.Languages.Expressions {
  public abstract class Expression {
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
  }
}