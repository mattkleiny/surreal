using Surreal.IO;

namespace Surreal.Scripting.Languages.Lox;

/// <summary>
/// The Lox language. A simple language that is similar to C.
/// <para/>
/// We will be using this language to test the scripting engine.
/// </summary>
public sealed class LoxScriptLanguage : IScriptLanguage
{
  public string Name => "Lox";

  public bool CanLoad(VirtualPath path)
  {
    return path.Extension.EndsWith("lox");
  }

  public async Task<ScriptModule> ParseAsync(TextReader reader)
  {
    var scanner = new Scanner(reader);
    var parser = new Parser(await scanner.ScanTokensAsync());

    return parser.ParseModule();
  }
}

file sealed class Scanner(TextReader reader)
{
  public Task<List<Token>> ScanTokensAsync()
  {
    throw new NotImplementedException();
  }
}

file sealed class Parser(List<Token> tokens)
{
  public ScriptModule ParseModule()
  {
    throw new NotImplementedException();
  }
}

file enum TokenType
{
  // Single-character tokens.
  LeftParen, RightParen, LeftBrace, RightBrace,
  Comma, Dot, Minus, Plus, Semicolon, Slash, Star,

  // One or two character tokens.
  Bang, BangEqual,
  Equal, EqualEqual,
  Greater, GreaterEqual,
  Less, LessEqual,

  // Literals.
  Identifier, String, Number,

  // Keywords.
  And, Class, Else, False, Fun, For, If, Nil, Or,
  Print, Return, Super, This, True, Var, While,

  Eof
}

file record struct Token(TokenType Type, string Lexeme, object Literal, int Line);

// statements
file abstract record Statement;
file sealed record ExpressionStatement(Expression Expression) : Statement;
file sealed record PrintStatement(Expression Expression) : Statement;
file sealed record VarStatement(string Name, Expression? Initializer) : Statement;
file sealed record BlockStatement(Block Block) : Statement;

// expressions
file abstract record Expression;
file sealed record Block(IReadOnlyList<Statement> Statements) : Expression;
file sealed record Binary(Expression Left, BinaryOp Operator, Expression Right) : Expression;
file sealed record Unary(UnaryOp Operator, Expression Expression) : Expression;
file sealed record Grouping(Expression Expression) : Expression;
file sealed record Literal(object Value) : Expression;
file sealed record Variable(string Name) : Expression;

// operators
file enum BinaryOp { Add, Subtract, Multiply, Divide }
file enum UnaryOp { Negate }
