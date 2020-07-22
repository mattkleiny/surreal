using System.Threading.Tasks;
using Surreal.Diagnostics.Console.Interpreter;
using Xunit;
using static Surreal.Diagnostics.Console.Interpreter.ConsoleExpression;

namespace Surreal.Jam.Diagnostics.Console {
  public class ConsoleParserTests {
    [Fact]
    public async Task it_should_parse_simple_expressions() {
      var parser     = await ConsoleParser.ParseAsync("print 'Hello, World!'");
      var expression = parser.Expression();

      Assert.IsType<CallExpression>(expression);
    }
  }
}