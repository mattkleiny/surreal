namespace Surreal.Terminals;

public class TerminalTests
{
  [Test, AutoFixture]
  public void it_should_write_characters(Terminal terminal)
  {
    terminal.WriteAt(0, 0, 'a');

    terminal.Received(1).DrawGlyph(0, 0, Arg.Any<Glyph>());
  }

  [Test, AutoFixture]
  public void it_should_write_strings(Terminal terminal)
  {
    terminal.Width.Returns(16);
    terminal.Height.Returns(16);

    terminal.WriteAt(0, 0, "Hello, World!");

    terminal.Received(13).DrawGlyph(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Glyph>());
  }

  [Test, AutoFixture]
  public void it_should_fill_terminal(Terminal terminal)
  {
    terminal.Width.Returns(16);
    terminal.Height.Returns(16);

    terminal.Fill(4, 4, 8, 8);

    terminal.Received(64).DrawGlyph(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Glyph>());
  }

  [Test, AutoFixture]
  public void it_should_clear_terminal(Terminal terminal)
  {
    terminal.Width.Returns(16);
    terminal.Height.Returns(16);

    terminal.Fill(4, 4, 8, 8);

    terminal.Received(64).DrawGlyph(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Glyph>());
  }
}
