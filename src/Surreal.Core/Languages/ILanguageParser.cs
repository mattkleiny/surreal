using System.Collections.Generic;
using System.Linq;
using Surreal.Languages.Statements;

namespace Surreal.Languages {
  public interface ILanguageParser {
    Statement ParseStatement(string raw) {
      return ParseStatements(raw).FirstOrDefault();
    }

    IEnumerable<Statement> ParseStatements(string raw);
  }
}