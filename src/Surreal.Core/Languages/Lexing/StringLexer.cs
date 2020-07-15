using System;
using System.Collections.Generic;
using Surreal.IO;

namespace Surreal.Languages.Lexing {
  public abstract class StringLexer<TToken> : Lexer<TToken>
      where TToken : struct {
    public override IEnumerable<TToken> Tokenize(SourceText text) {
      var results = new List<TToken>();
      var buffer  = text.Span;

      var lineNumber   = 0;
      var columnNumber = 0;

      while (buffer.Length > 0) {
        var line    = buffer.Split('\n');
        var segment = line;

        while (segment.Length > 0) {
          var position = new TokenPosition(lineNumber, columnNumber);

          if (!TryMatch(segment, position, out var token, out var length, out var ignore)) {
            throw new LexingException($"An unrecognized token was encountered: {segment.ToString()}", position);
          }

          if (!ignore) {
            results.Add(token);
          }

          segment      =  segment.Slice(length);
          columnNumber += length;
        }

        lineNumber++;
        columnNumber = 0;
      }

      return results;
    }

    protected abstract bool TryMatch(
        ReadOnlySpan<char> characters,
        TokenPosition position,
        out TToken token,
        out int length,
        out bool ignore
    );
  }
}