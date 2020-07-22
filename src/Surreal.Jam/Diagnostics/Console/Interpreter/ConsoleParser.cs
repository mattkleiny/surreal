using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Surreal.Text.Lexing;
using Surreal.Text.Parsing;
using static Surreal.Diagnostics.Console.Interpreter.ConsoleExpression;

namespace Surreal.Diagnostics.Console.Interpreter {
  public sealed class ConsoleParser : DescentParser<ConsoleParser.TokenType> {
    private static readonly RegexLexer<Token> Lexer = new RegexLexer<Token>(
        Rule(@"\s+", lexeme => (TokenType.WhiteSpace, null), disregard: true),
        Rule(@"\+|\-|\*|\/", lexeme => (TokenType.Operator, null)),
        Rule(@"\d+", lexeme => (TokenType.Number, float.Parse(lexeme))),
        Rule(@"'[A-Za-z0-9]'", lexeme => (TokenType.String, null))
    );

    public static Task<ConsoleParser> ParseAsync(string raw) {
      return ParseAsync(new StringReader(raw));
    }

    public static async Task<ConsoleParser> ParseAsync(TextReader reader) {
      return new ConsoleParser(await Lexer.TokenizeAsync(reader));
    }

    private ConsoleParser(IEnumerable<Token> tokens)
        : base(tokens) {
    }

    public ConsoleExpression Expression() {
      throw new NotImplementedException();
    }

    private CallExpression CallExpression() {
      throw new NotImplementedException();
    }

    private UnaryExpression UnaryExpression() {
      var operation  = UnaryOperator();
      var expression = LiteralExpression();

      return new UnaryExpression(operation, expression);
    }

    private BinaryExpression BinaryExpression() {
      var left      = LiteralExpression();
      var operation = BinaryOperator();
      var right     = LiteralExpression();

      return new BinaryExpression(operation, left, right);
    }

    private LiteralExpression LiteralExpression() {
      if (Match(TokenType.Number)) return new LiteralExpression(Peek().Lexeme);
      if (Match(TokenType.String)) return new LiteralExpression(Peek().Lexeme);

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