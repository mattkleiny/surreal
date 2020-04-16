using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Surreal.Languages.Lexing
{
  public abstract class Lexer<TToken>
    where TToken : struct
  {
    public IEnumerable<TToken> Tokenize(string input)
    {
      return TokenizeAsync(new StringReader(input)).Result;
    }

    public abstract Task<IEnumerable<TToken>> TokenizeAsync(TextReader reader);
  }
}
