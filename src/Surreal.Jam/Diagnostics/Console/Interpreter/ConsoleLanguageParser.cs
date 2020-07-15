using System;
using Surreal.Languages;
using Surreal.Languages.Lexing;
using Surreal.Languages.Parsing;
using static Surreal.Diagnostics.Console.Interpreter.ConsoleExpression;

namespace Surreal.Diagnostics.Console.Interpreter {
  public sealed class ConsoleLanguageParser : DescentParser<ConsoleLanguageParser.TokenType> {
    private static readonly RegexLexer<Token> Lexer = new RegexLexer<Token>(
        new RegexLexer<Token>.Rule(@"\s+", (lexeme, position) => new Token(TokenType.WhiteSpace, position, lexeme), disregard: true),
        new RegexLexer<Token>.Rule(@"\+|\-|\*|\/", (lexeme, position) => new Token(TokenType.Operator, position, lexeme)),
        new RegexLexer<Token>.Rule(@"\d+", (lexeme, position) => new Token(TokenType.Number, position, float.Parse(lexeme))),
        new RegexLexer<Token>.Rule(@"'[A-Za-z0-9]'", (lexeme, position) => new Token(TokenType.String, position, lexeme.Trim('.')))
    );

    public ConsoleLanguageParser(SourceText text)
        : base(Lexer.Tokenize(text)) {
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
      if (Match(TokenType.Number)) return new Literal(Peek().Lexeme);
      if (Match(TokenType.String)) return new Literal(Peek().Lexeme);

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

    public enum TokenType {
      WhiteSpace,
      Operator,
      Number,
      String,
    }
  }
}