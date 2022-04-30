using Antlr4.Runtime;
using Surreal.Graphics.Shaders.Internal;

namespace Surreal.Graphics.Shaders;

internal static class ShadeLanguage
{
  public static ShaderSyntaxTree Parse(string raw)
  {
    var stream = new AntlrInputStream(raw);
    var lexer = new ShadeLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new ShadeParser(tokens);

    parser.AddErrorListener(new ConsoleErrorListener<IToken>());

    return parser.expression().Accept(new SyntaxTreeTransformer());
  }

  private sealed class SyntaxTreeTransformer : ShadeBaseVisitor<ShaderSyntaxTree>
  {
  }
}
