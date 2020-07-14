using System;
using System.Collections.Generic;

namespace Surreal.Languages.Lexing {
  public abstract class StringLexer<TToken> : Lexer<TToken>
      where TToken : struct {
    protected abstract bool TryMatch(
        ReadOnlySpan<char> characters,
        TokenPosition position,
        out TToken token,
        out int length,
        out bool ignore
    );

    public override IEnumerable<TToken> Tokenize(SourceText text) {
      var span = text.Span;

      for (var start = 0; start < span.Length; start++)
      for (var end = start; end < span.Length; end++) {
        if (span[end] != '\n') continue; // find the end of hte line

        var line     = span[start..end];
        var position = new TokenPosition(0, 0);

        if (TryMatch(line, position, out var token, out var length, out var ignore)) {
          if (!ignore) {
            yield return token;
          }

          start += length;
        } else {
          throw new LexingException($"An unrecognized token was encountered: {line.ToString()}");
        }
      }
    }
  }
}