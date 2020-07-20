using System;
using System.Collections.Generic;
using static Surreal.Graphics.Materials.Shaders.ShaderExpression;
using static Surreal.Graphics.Materials.Shaders.ShaderStatement;

namespace Surreal.Graphics.Materials.Shaders {
  public abstract class ShaderCompiler {
    public abstract Shader Compile(
        ShaderProgramType programType,
        ShaderType shaderType,
        IEnumerable<UniformDeclaration> uniforms,
        IEnumerable<FunctionDeclaration> functions
    );
  }

  public sealed class SpirvShaderCompiler : ShaderCompiler {
    public override Shader Compile(
        ShaderProgramType programType,
        ShaderType shaderType,
        IEnumerable<UniformDeclaration> uniforms,
        IEnumerable<FunctionDeclaration> functions) {
      var builder     = new ModuleBuilder(programType);
      var transformer = new Transformer(builder);

      foreach (var function in functions) {
        function.Accept(transformer);
      }

      var module = builder.Build();

      return new Shader(shaderType, module.Compile());
    }

    private sealed class Module {
      public Memory<byte> Compile() {
        throw new NotImplementedException();
      }
    }

    private sealed class ModuleBuilder {
      public ModuleBuilder(ShaderProgramType programType) {
        throw new NotImplementedException();
      }

      public Module Build() {
        throw new NotImplementedException();
      }
    }

    private abstract class Instruction {
    }

    private sealed class Transformer : ShaderStatement.IVisitor<Instruction[]>, ShaderExpression.IVisitor<Instruction[]> {
      private readonly ModuleBuilder builder;

      public Transformer(ModuleBuilder builder) {
        this.builder = builder;
      }

      public Instruction[] Visit(MetadataDeclaration statement) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(ShaderDeclaration statement) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(FunctionDeclaration statement) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(UniformDeclaration statement) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(LiteralExpression expression) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(UnaryExpression expression) {
        throw new NotImplementedException();
      }

      public Instruction[] Visit(BinaryExpression expression) {
        throw new NotImplementedException();
      }
    }
  }
}