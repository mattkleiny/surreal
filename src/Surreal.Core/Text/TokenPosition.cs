namespace Surreal.Text {
  public readonly struct TokenPosition {
    public readonly int Line;
    public readonly int Column;

    public TokenPosition(int line, int column) {
      Line   = line;
      Column = column;
    }

    public void Deconstruct(out int line, out int column) {
      line   = Line;
      column = Column;
    }

    public override string ToString() => $"{Line}:{Column}";
  }
}