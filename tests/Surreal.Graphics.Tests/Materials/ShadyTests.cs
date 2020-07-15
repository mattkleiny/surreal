using System.Threading.Tasks;
using Surreal.Graphics.Materials.Shady;
using Surreal.Languages;
using Xunit;

namespace Surreal.Graphics.Materials {
  public class ShadyTests {
    private static readonly SourceText TestProgram = SourceText.FromString(@"
      #type sprite
      #description A simple program for rendering sprite geometry

      void fragment() {
        COLOR = float4(1, 1, 1, 1);
      }
");

    [Fact]
    public async Task it_should_parse_a_simple_program() {
      var program = await ShadyProgram.ParseAsync(TestProgram);

      Assert.Equal(ShadyProgramType.Sprite, program.Metadata.Type);
      Assert.Single(program.Shaders);
    }
  }
}