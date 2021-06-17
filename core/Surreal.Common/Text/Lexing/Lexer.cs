using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Surreal.Text.Lexing {
  public abstract class Lexer<TToken>
      where TToken : struct {
    public abstract Task<IEnumerable<TToken>> TokenizeAsync(TextReader reader);
  }
}