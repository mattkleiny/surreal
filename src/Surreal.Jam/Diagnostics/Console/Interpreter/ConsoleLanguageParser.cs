using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Languages;
using Surreal.Languages.Lexing;
using static Surreal.Diagnostics.Console.Interpreter.ConsoleExpression;

namespace Surreal.Diagnostics.Console.Interpreter {
  public sealed class ConsoleLanguageParser {
    private static readonly RegexLexer<Token> Lexer = new RegexLexer<Token>(
        new RegexLexer<Token>.Rule(@"\s+", (lexeme, position) => new Token(TokenType.WhiteSpace, position, lexeme), disregard: true),
        new RegexLexer<Token>.Rule(@"\+|\-|\*|\/", (lexeme, position) => new Token(TokenType.Operator, position, lexeme)),
        new RegexLexer<Token>.Rule(@"\d+", (lexeme, position) => new Token(TokenType.Number, position, float.Parse(lexeme))),
        new RegexLexer<Token>.Rule(@"'[A-Za-z0-9]'", (lexeme, position) => new Token(TokenType.String, position, lexeme.Trim('.')))
    );

    private readonly Token[] tokens;
    private          int     position;

    private Token CurrentToken => tokens[position - 1];
    private Token NextToken    => tokens[position];
    private bool  IsAtEnd      => position >= tokens.Length;

    public ConsoleLanguageParser(SourceText text) {
      tokens = Lexer.Tokenize(text).ToArray();
    }

    public ConsoleExpression Expression() {
      throw new NotImplementedException();
    }

    private Call CallExpression() {
      throw new NotImplementedException();
    }

    private Unary UnaryExpression() {
      var operation  = UnaryOperator();
      var expression = Literal();

      return new Unary(operation, expression);
    }

    private Binary BinaryExpression() {
      var left      = Literal();
      var operation = BinaryOperator();
      var right     = Literal();

      return new Binary(operation, left, right);
    }

    private Literal Literal() {
      if (Match(TokenType.Number)) return new Literal(CurrentToken.Lexeme);
      if (Match(TokenType.String)) return new Literal(CurrentToken.Lexeme);

      throw Error();
    }

    private BinaryOperation BinaryOperator() => Consume(TokenType.Operator).Lexeme switch {
        "+" => BinaryOperation.Plus,
        "-" => BinaryOperation.Minus,
        "*" => BinaryOperation.Times,
        "/" => BinaryOperation.Divide,
        _   => throw Error()
    };

    private UnaryOperation UnaryOperator() => Consume(TokenType.Operator).Lexeme switch {
        "!" => UnaryOperation.Not,
        "-" => UnaryOperation.Negate,
        _   => throw Error()
    };

    private Token Advance() {
      if (!IsAtEnd) position++;

      return CurrentToken;
    }

    private Token Consume(TokenType type, [CallerMemberName] string? expected = null) {
      if (Check(type)) return Advance();

      throw Error(expected);
    }

    private bool Check(TokenType type) {
      if (IsAtEnd) return false;

      return NextToken.Type == type;
    }

    private bool Match(params TokenType[] types) {
      for (var i = 0; i < types.Length; i++) {
        if (Check(types[i])) {
          Advance();
          return true;
        }
      }

      return false;
    }

    private Exception Error([CallerMemberName] string? expected = null) {
      var token = NextToken;

      var position = token.Position;
      var lexeme   = token.Lexeme;

      return new Exception($"{expected} at {position} (got {lexeme} instead)");
    }

    private enum TokenType {
      WhiteSpace,
      Operator,
      Number,
      String,
    }

    private readonly struct Token {
      public readonly TokenType     Type;
      public readonly TokenPosition Position;
      public readonly object        Lexeme;

      public Token(TokenType type, TokenPosition position, object lexeme) {
        Lexeme   = lexeme;
        Type     = type;
        Position = position;
      }
    }
  }
}