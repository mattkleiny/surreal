namespace Surreal.Text.Parsing;

public readonly struct TokenPosition
{
  public readonly int Line;
  public readonly int Column;

  public TokenPosition(int line, int column)
  {
    Line   = line;
    Column = column;
  }

  public void Deconstruct(out int line, out int column)
  {
    line   = Line;
    column = Column;
  }

  public override string ToString() => $"{Line.ToString()}:{Column.ToString()}";
}

public sealed class LexingException : Exception
{
  public LexingException(string message, in TokenPosition position)
    : base(message)
  {
    Position = position;
  }

  public TokenPosition Position { get; }
}

public abstract class Lexer<TToken>
  where TToken : struct
{
  public abstract Task<IEnumerable<TToken>> TokenizeAsync(TextReader reader);
}