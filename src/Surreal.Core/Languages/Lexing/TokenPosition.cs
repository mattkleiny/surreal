namespace Surreal.Languages.Lexing {
  public readonly struct TokenPosition {
    public readonly int Line;
    public readonly int Column;

    public TokenPosition(int line, int column) {
      Line   = line;
      Column = column;
    }

    public override string ToString() => $"{Line}:{Column}";
  }
}