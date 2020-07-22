using Surreal.Framework.Blueprints;
using Xunit;

namespace Surreal.Jam.Framework {
  public class BlueprintTests {
    [Fact]
    public void it_should_parse_a_simple_blueprint() {
      var blueprint = Blueprint.Parse("@blueprint 'Name'");

      Assert.NotNull(blueprint);
    }
  }
}