using System;
using System.Collections.Generic;
using static Surreal.Graphics.Materials.Shaders.ShaderExpression;
using static Surreal.Graphics.Materials.Shaders.ShaderStatement;

namespace Surreal.Graphics.Materials.Shaders.Spirv {
  public sealed class SpirvShaderCompiler : IShaderCompiler {
    public Shader Compile(
        ShaderProgramType programType,
        ShaderType shaderType,
        IEnumerable<UniformDeclaration> uniforms,
        IEnumerable<FunctionDeclaration> functions) {
      var module   = BuildModule(programType, shaderType, uniforms, functions);
      var bytecode = module.Compile();

      return new Shader(shaderType, bytecode);
    }

    private static SpirvModule BuildModule(
        ShaderProgramType programType,
        ShaderType shaderType,
        IEnumerable<UniformDeclaration> uniforms,
        IEnumerable<FunctionDeclaration> functions) {
      var module      = new SpirvModule();
      var transformer = new Transformer(module);

      foreach (var uniform in uniforms) {
        uniform.Accept(transformer);
      }

      foreach (var function in functions) {
        function.Accept(transformer);
      }

      return module;
    }

    private sealed class Transformer : ShaderStatement.IVisitor<Instruction[]>, ShaderExpression.IVisitor<Instruction[]> {
      private readonly SpirvModule module;

      public Transformer(SpirvModule module) {
        this.module = module;
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