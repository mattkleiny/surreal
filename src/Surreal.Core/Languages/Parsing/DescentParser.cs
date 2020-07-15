using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Utilities;

namespace Surreal.Languages.Parsing {
  public abstract class DescentParser<TToken, TTokenType>
      where TToken : struct {
    private readonly TToken[] tokens;
    private          int      current;

    protected DescentParser(IEnumerable<TToken> tokens) {
      this.tokens = tokens.ToArray();
    }

    protected abstract bool          IsTokenOfType(TToken token, TTokenType type);
    protected abstract TokenPosition GetTokenPosition(TToken token);
    protected abstract object        GetTokenLexeme(TToken token);

    protected TToken Peek()     => tokens[current];
    protected TToken Previous() => tokens[current - 1];
    protected bool   IsAtEnd()  => current >= tokens.Length;

    protected TToken Advance() {
      if (!IsAtEnd()) current++;

      return Previous();
    }

    protected bool Check(TTokenType type) {
      if (IsAtEnd()) return false;

      return IsTokenOfType(Peek(), type);
    }

    protected bool Match(params TTokenType[] types) {
      for (var i = 0; i < types.Length; i++) {
        if (Check(types[i])) {
          Advance();
          return true;
        }
      }

      return false;
    }

    protected TToken Consume(TTokenType type, [CallerMemberName] string? expected = null) {
      if (Check(type)) return Advance();

      throw Error(expected);
    }

    protected Exception Error([CallerMemberName] string? expected = null) {
      var token = Peek();

      var position = GetTokenPosition(token);
      var lexeme   = GetTokenLexeme(token);

      return new ParsingException($"{expected} at {position} (got {lexeme} instead)");
    }
  }

  public abstract class DescentParser<TTokenType> : DescentParser<DescentParser<TTokenType>.Token, TTokenType>
      where TTokenType : unmanaged, Enum {
    protected DescentParser(IEnumerable<Token> tokens)
        : base(tokens) {
    }

    protected sealed override bool          IsTokenOfType(Token token, TTokenType type) => token.Type.EqualsFast(type);
    protected sealed override TokenPosition GetTokenPosition(Token token)               => token.Position;
    protected sealed override object        GetTokenLexeme(Token token)                 => token.Lexeme;

    public readonly struct Token {
      public readonly TTokenType    Type;
      public readonly TokenPosition Position;
      public readonly object        Lexeme;

      public Token(TTokenType type, TokenPosition position, object lexeme) {
        Lexeme   = lexeme;
        Type     = type;
        Position = position;
      }

      public void Deconstruct(out TTokenType type, out TokenPosition position, [NotNull] out object lexeme) {
        type     = Type;
        position = Position;
        lexeme   = Lexeme;
      }

      public override string ToString() => $"{Type} at {Position} ({Lexeme})";
    }
  }
}