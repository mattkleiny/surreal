using Surreal.Graphics.Shaders;
using static Surreal.Graphics.Shaders.ShaderInstruction.Expression;
using static Surreal.Graphics.Shaders.ShaderInstruction.Statement;

namespace Surreal.Internal.Graphics;

public class OpenTkShaderCompilerTests
{
  [Test, Benchmark(Milliseconds = 0.5f)]
  public async Task it_should_compile_simple_instructions()
  {
    var compiler = new OpenTkShaderCompiler();

    var program = await compiler.CompileAsync(new ShaderProgramDeclaration(
      FileName: "test.shader",
      Description: "A simple test program",
      Archetype: ShaderArchetype.Sprite,

      // vertex shader
      new ShaderDeclaration(
        ShaderKind.Vertex,
        new UniformDeclaration(new Primitive(PrimitiveType.Float), "_Amount"),
        new UniformDeclaration(new Primitive(PrimitiveType.Float, 2), "_Position"),
        new BlankLine(),
        new VaryingDeclaration(new Primitive(PrimitiveType.Float, 4, Precision.Low), "_Color"),
        new BlankLine(),
        new FunctionDeclaration(
          new Primitive(PrimitiveType.Void),
          "vertex",
          new Assignment("_Color",
            new Binary(
              ShaderInstruction.BinaryOperator.Mul,
              new Constant("_Position"),
              new Constructor(
                new Primitive(PrimitiveType.Float, 4),
                new Variadic(new Constant(1), new Constant(1), new Constant(1), new Constant(1))
              )
            )
          )
        )
      ),

      // fragment shader
      new ShaderDeclaration(
        ShaderKind.Fragment,
        new VaryingDeclaration(new Primitive(PrimitiveType.Float, 4), "_Color"),
        new BlankLine(),
        new ConstantDeclaration(new Primitive(PrimitiveType.Bool), "IS_FULL_SCREEN", new Constant("true")),
        new BlankLine(),
        new FunctionDeclaration(
          new Primitive(PrimitiveType.Void),
          "fragment",
          new Parameter(new Primitive(PrimitiveType.Bool), "use_full_screen"),
          new IntrinsicAssignment(IntrinsicType.Color, new Constant("_Color"))
        )
      )
    ));

    Assert.IsNotNull(program);
  }
}
