using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Collections;

namespace Surreal.Text.Parsing
{
  public sealed class ParsingException : Exception
  {
    public ParsingException()
    {
    }

    public ParsingException(string message)
        : base(message)
    {
    }

    public ParsingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
  }

  public abstract class DescentParser<TToken, TTokenType>
      where TToken : struct
  {
    private readonly TToken[] tokens;
    private          int      current;

    protected DescentParser(IEnumerable<TToken> tokens)
    {
      this.tokens = tokens.ToArray();
    }

    protected abstract bool          IsTokenOfType(TToken token, TTokenType type);
    protected abstract TokenPosition GetTokenPosition(TToken token);
    protected abstract string        GetTokenLexeme(TToken token);

    protected TToken Peek()     => tokens[current];
    protected TToken Previous() => tokens[current - 1];
    protected bool   IsAtEnd()  => current >= tokens.Length;

    protected TToken Advance()
    {
      if (!IsAtEnd()) current++;

      return Previous();
    }

    protected bool Check(TTokenType type)
    {
      if (IsAtEnd()) return false;

      return IsTokenOfType(Peek(), type);
    }

    protected bool Match(params TTokenType[] types)
    {
      for (var i = 0; i < types.Length; i++)
      {
        if (Check(types[i]))
        {
          Advance();
          return true;
        }
      }

      return false;
    }

    protected TToken Consume(TTokenType type, [CallerMemberName] string? expected = null)
    {
      if (Check(type)) return Advance();

      throw Error(expected);
    }

    protected Exception Error([CallerMemberName] string? expected = null)
    {
      var token = Peek();

      var position = GetTokenPosition(token);
      var lexeme   = GetTokenLexeme(token);

      return new ParsingException($"{expected} at {position} (got {lexeme} instead)");
    }
  }

  public abstract class DescentParser<TTokenType> : DescentParser<DescentParser<TTokenType>.Token, TTokenType>
      where TTokenType : unmanaged, Enum
  {
    protected DescentParser(IEnumerable<Token> tokens)
        : base(tokens)
    {
    }

    protected sealed override bool          IsTokenOfType(Token token, TTokenType type) => token.Type.EqualsFast(type);
    protected sealed override TokenPosition GetTokenPosition(Token token)               => token.Position;
    protected sealed override string        GetTokenLexeme(Token token)                 => token.Lexeme;

    public readonly struct Token
    {
      public readonly TTokenType    Type;
      public readonly TokenPosition Position;
      public readonly string        Lexeme;
      public readonly object?       Literal;

      public Token(TTokenType type, TokenPosition position, string lexeme, object? literal = null)
      {
        Type     = type;
        Lexeme   = lexeme;
        Position = position;
        Literal  = literal;
      }

      public void Deconstruct(out TTokenType type, out TokenPosition position, out string lexeme, out object? literal)
      {
        type     = Type;
        position = Position;
        lexeme   = Lexeme;
        literal  = Literal;
      }

      public override string ToString() => $"{Type} at {Position.ToString()} ({Lexeme})";
    }

    protected static RegexLexer<Token>.Rule Rule(string pattern, Func<string, (TTokenType Type, object? Literal)> factory, bool disregard = false)
    {
      return new(
          pattern: pattern,
          disregard: disregard,
          tokenizer: (lexeme, position) =>
          {
            var (type, literal) = factory(lexeme);
            return new Token(type, position, lexeme, literal);
          }
      );
    }
  }
}