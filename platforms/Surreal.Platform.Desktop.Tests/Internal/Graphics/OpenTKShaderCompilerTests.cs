using Surreal.Graphics.Shaders;
using Surreal.IO;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Internal.Graphics;

public class OpenTKShaderCompilerTests
{
  private static readonly Version Version = new(3, 3, 0);

  [Test]
  public void it_should_compile_simple_instructions()
  {
    var compiler = new OpenTKShaderCompiler(Version);

    var program = compiler.CompileShader(new ShaderDeclaration(
      Path: "test.shade",
      CompilationUnit: new ShaderCompilationUnit
      {
        Uniforms = ImmutableArray.Create(
          new UniformDeclaration(new Primitive(PrimitiveType.Float), "_Amount"),
          new UniformDeclaration(new Primitive(PrimitiveType.Float, 2), "_Position")
        ),
        Varyings = ImmutableArray.Create(
          new VaryingDeclaration(new Primitive(PrimitiveType.Float, 4, Precision.Low), "_Color")
        ),
        Constants = ImmutableArray.Create(
          new ConstantDeclaration(new Primitive(PrimitiveType.Bool), "IS_HIGH_PRECISION", new Constant("true"))
        ),
        Stages = ImmutableArray.Create(
          // vertex program
          new StageDeclaration(ShaderKind.Vertex)
          {
            Statements = ImmutableArray.Create<Statement>(
              new Assignment("_Color",
                new BinaryOperation(
                  BinaryOperator.Multiply,
                  new Constant("_Position"),
                  new TypeConstructor(
                    new Primitive(PrimitiveType.Float, 4),
                    new Variadic(new Constant(1), new Constant(0), new Constant(1), new Constant(1))
                  )
                )
              )
            ),
          },

          // fragment program
          new StageDeclaration(ShaderKind.Fragment)
          {
            Statements = ImmutableArray.Create<Statement>(
              new Assignment("COLOR", new SampleOperation("_Texture", new Constant("input.xy")))
            ),
          }
        ),
      }
    ));

    program.Should().NotBeNull();
    program.Shaders.Length.Should().Be(2);
    program.Shaders[0].Code.Should().NotBeEmpty();
    program.Shaders[1].Code.Should().NotBeEmpty();
  }

  [Test]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/common.shade")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/geometry.shade")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/sprite.shade")]
  public async Task it_should_compile_shader_programs(VirtualPath path)
  {
    var parser = new ShaderParser();
    var compiler = new OpenTKShaderCompiler(Version);

    var declaration = await parser.ParseAsync(path);
    var shaderSet = compiler.CompileShader(declaration);

    shaderSet.Should().NotBeNull();

    foreach (var shader in shaderSet.Shaders)
    {
      Console.WriteLine(shader.Code);
    }
  }
}
