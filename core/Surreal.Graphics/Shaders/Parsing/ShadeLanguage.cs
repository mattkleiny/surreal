using Antlr4.Runtime;
using Surreal.Graphics.Shaders.Internal;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders.Parsing;

/// <summary>Utilities for working with the Shade language.</summary>
internal static class ShadeLanguage
{
  /// <summary>Parses the given string into a <see cref="ShaderCompilationUnit"/>.</summary>
  public static ShaderCompilationUnit Parse(string raw)
  {
    var stream = new AntlrInputStream(raw);
    var lexer = new ShadeLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new ShadeParser(tokens);

    parser.AddErrorListener(new ConsoleErrorListener<IToken>());

    return (ShaderCompilationUnit) parser.program().Accept(new SyntaxTreeTransformer());
  }

  /// <summary>A <see cref="ShadeBaseVisitor{Result}"/> that converts from an Antlr parse tree into our internal AST.</summary>
  private sealed class SyntaxTreeTransformer : ShadeBaseVisitor<ShaderSyntaxTree>
  {
    public override ShaderSyntaxTree VisitProgram(ShadeParser.ProgramContext context)
    {
      var result = new ShaderCompilationUnit();
      var declarations = context.declaration();

      foreach (var declaration in declarations)
      {
        var declarationTree = VisitDeclaration(declaration);

        switch (declarationTree)
        {
          case UniformDeclaration uniform:
            result = result with { Uniforms = result.Uniforms.Add(uniform) };
            break;

          case VaryingDeclaration varying:
            result = result with { Varyings = result.Varyings.Add(varying) };
            break;
        }
      }

      return result;
    }

    public override ShaderSyntaxTree VisitUniformDeclaration(ShadeParser.UniformDeclarationContext context)
    {
      var primitive = (PrimitiveDeclaration) VisitPrimitiveDeclaration(context.primitiveDeclaration());

      return new UniformDeclaration(primitive);
    }

    public override ShaderSyntaxTree VisitVaryingDeclaration(ShadeParser.VaryingDeclarationContext context)
    {
      var primitive = (PrimitiveDeclaration) VisitPrimitiveDeclaration(context.primitiveDeclaration());

      return new VaryingDeclaration(primitive);
    }

    public override ShaderSyntaxTree VisitPrimitiveDeclaration(ShadeParser.PrimitiveDeclarationContext context)
    {
      Precision? precision = context.PRECISION().GetText() switch
      {
        "lowp"  => Precision.Low,
        "medp"  => Precision.Medium,
        "highp" => Precision.High,
        _       => null
      };

      Primitive primitive = context.PRIMITIVE().GetText() switch
      {
        "void"      => new Primitive(PrimitiveType.Void, null, precision),
        "bool"      => new Primitive(PrimitiveType.Bool, null, precision),
        "bool2"     => new Primitive(PrimitiveType.Bool, 2, precision),
        "bool3"     => new Primitive(PrimitiveType.Bool, 3, precision),
        "bool4"     => new Primitive(PrimitiveType.Bool, 4, precision),
        "int"       => new Primitive(PrimitiveType.Int, null, precision),
        "int2"      => new Primitive(PrimitiveType.Int, 2, precision),
        "int3"      => new Primitive(PrimitiveType.Int, 3, precision),
        "int4"      => new Primitive(PrimitiveType.Int, 4, precision),
        "float"     => new Primitive(PrimitiveType.Float, null, precision),
        "float2"    => new Primitive(PrimitiveType.Float, 2, precision),
        "float3"    => new Primitive(PrimitiveType.Float, 3, precision),
        "float4"    => new Primitive(PrimitiveType.Float, 4, precision),
        "vec2"      => new Primitive(PrimitiveType.Float, 2, precision),
        "vec3"      => new Primitive(PrimitiveType.Float, 3, precision),
        "vec4"      => new Primitive(PrimitiveType.Float, 4, precision),
        "sampler1d" => new Primitive(PrimitiveType.Sampler, 1),
        "sampler2d" => new Primitive(PrimitiveType.Sampler, 2),
        "sampler3d" => new Primitive(PrimitiveType.Sampler, 3),

        _ => throw new InvalidOperationException($"An unrecognized type was specified: {context.PRIMITIVE().GetText()}")
      };

      var name = context.IDENTIFIER().GetText();

      return new PrimitiveDeclaration(primitive, name);
    }
  }
}
