using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Internal.Graphics;

public class OpenTKShaderCompilerTests
{
  [Test, Benchmark(ThresholdMs = 0.2f)]
  public async Task it_should_compile_simple_instructions()
  {
    var compiler = new OpenTKShaderCompiler();

    var program = (OpenTKShaderSet) await compiler.CompileAsync(
      new ShaderProgramDeclaration(
        Path: "test.shader",
        Archetype: ShaderArchetype.Sprite,
        new CompilationUnit(
          // shared globals
          new UniformDeclaration(new Primitive(PrimitiveType.Float), "_Amount"),
          new UniformDeclaration(new Primitive(PrimitiveType.Float, 2), "_Position"),
          new VaryingDeclaration(new Primitive(PrimitiveType.Float, 4, Precision.Low), "_Color"),
          new ConstantDeclaration(new Primitive(PrimitiveType.Bool), "IS_HIGH_PRECISION", new Constant("true")),

          // vertex stage
          new StageDeclaration(
            ShaderKind.Vertex,
            new Assignment("_Color",
              new Binary(
                BinaryOperator.Mul,
                new Constant("_Position"),
                new Constructor(
                  new Primitive(PrimitiveType.Float, 4),
                  new Variadic(new Constant(1), new Constant(1), new Constant(1), new Constant(1))
                )
              )
            )
          ),

          // fragment stage
          new StageDeclaration(
            ShaderKind.Fragment,
            new IntrinsicAssignment(IntrinsicType.Color, new Constant("_Color"))
          )
        )
      )
    );

    Assert.IsNotNull(program);
    Assert.AreEqual(2, program.Shaders.Length);
    Assert.That(program.Shaders[0].Code, Is.Not.Empty);
    Assert.That(program.Shaders[1].Code, Is.Not.Empty);
  }

  [Test, Benchmark(ThresholdMs = 0.2f)]
  public async Task it_should_compile_simple_program_from_standard_language()
  {
    IShaderParser parser   = new StandardShaderParser();
    var           compiler = new OpenTKShaderCompiler();

    var declaration = await parser.ParseShaderAsync(
      path: "test.shader",
      sourceCode: @"
      // A simple sprite shader, for testing purposes.
      // This description should be attached up-front.

      shader_type sprite;

      uniform vec3  _Position;
      uniform float _Intensity;
      varying vec3  _Color;

      void vertex()
      {
        _Color = vec3(1,1,1) * _Position * _Intensity;
      }

      void fragment()
      {
        COLOR = _Color;
      }
    ");

    var program = await compiler.CompileAsync(declaration);

    Assert.IsNotNull(program);
  }
}
