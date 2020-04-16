using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Surreal.Languages.Lexing
{
  public abstract class StringLexer<TToken> : Lexer<TToken>
    where TToken : struct
  {
    public override async Task<IEnumerable<TToken>> TokenizeAsync(TextReader reader)
    {
      string line;

      var currentLine = 1;
      var results     = new List<TToken>();

      while ((line = await reader.ReadLineAsync()) != null)
      {
        if (!string.IsNullOrEmpty(line))
        {
          var currentColumn = 0;

          while (currentColumn < line.Length)
          {
            var position = new TokenPosition(currentLine, currentColumn);

            if (TryMatch(line, position, out var token, out var length, out var ignore))
            {
              if (!ignore)
              {
                results.Add(token);
              }

              currentColumn += length;
            }
            else
            {
              throw new LexingException($"Unrecognized symbol {line[currentColumn]} at line {currentLine}, column {currentColumn}");
            }
          }

          currentLine += 1;
        }
      }

      return results;
    }

    protected abstract bool TryMatch(string line, TokenPosition position, out TToken token, out int length, out bool ignore);
  }
}
