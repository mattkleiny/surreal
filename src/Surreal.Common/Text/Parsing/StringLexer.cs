namespace Surreal.Text.Parsing;

public abstract class StringLexer<TToken> : Lexer<TToken>
  where TToken : struct
{
  public override Task<IEnumerable<TToken>> TokenizeAsync(TextReader reader)
  {
    throw new NotImplementedException();
  }

  protected abstract bool TryMatch(
    ReadOnlySpan<char> characters,
    TokenPosition position,
    out TToken token,
    out int length,
    out bool ignore
  );
}