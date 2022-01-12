namespace Surreal.Terminals;

public class TerminalSliceTests
{
  [Test, AutoFixture]
  public void it_should_write_characters_to_parent_terminal(Terminal terminal)
  {
    terminal.Width.Returns(16);
    terminal.Height.Returns(16);

    var slice = new TerminalSlice(4, 4, 8, 8, terminal);

    slice.Clear();

    terminal.Received(64).DrawGlyph(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Glyph>());
  }
}
