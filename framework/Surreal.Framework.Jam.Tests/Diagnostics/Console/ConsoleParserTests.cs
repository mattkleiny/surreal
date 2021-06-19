using System.Threading.Tasks;
using Surreal.Diagnostics.Console;
using Xunit;
using static Surreal.Diagnostics.Console.GameConsoleExpression;

namespace Surreal.Jam.Diagnostics.Console
{
  public class ConsoleParserTests
  {
    [Fact]
    public async Task it_should_parse_simple_expressions()
    {
      var parser     = await GameConsoleParser.ParseAsync("print 'Hello, World!'");
      var expression = parser.Expression();

      Assert.IsType<CallExpression>(expression);
    }
  }
}