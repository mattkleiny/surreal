using Antlr4.Runtime;
using Surreal.Graphics.Shaders.Internal;

namespace Surreal.Graphics.Shaders;

internal static class ShadeLanguage
{
  public static Unit Parse(string raw)
  {
    var stream = new AntlrInputStream(raw);
    var lexer = new ShadeLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new ShadeParser(tokens);

    parser.AddErrorListener(new ConsoleErrorListener<IToken>());
    parser.AddErrorListener(new ErrorListener());

    return parser.expression().Accept(new TransformVisitor());
  }

  private sealed class ErrorListener : BaseErrorListener
  {
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
    }
  }

  private sealed class TransformVisitor : ShadeBaseVisitor<Unit>
  {
    public override Unit VisitExpression(ShadeParser.ExpressionContext context)
    {
      return base.VisitExpression(context);
    }
  }
}
