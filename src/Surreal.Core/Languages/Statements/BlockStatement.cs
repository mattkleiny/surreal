using System.Collections.Generic;
using Surreal.Languages.Visitors;

namespace Surreal.Languages.Statements
{
  public sealed class BlockStatement : Statement
  {
    public IEnumerable<Statement> Statements { get; }

    public BlockStatement(IEnumerable<Statement> statements)
    {
      Statements = statements;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
