using System.Collections.Generic;
using System.IO;

namespace Surreal.Languages.Lexing {
  public abstract class Lexer<TToken>
      where TToken : struct {
    public abstract IEnumerable<TToken> Tokenize(SourceText text);
  }
}