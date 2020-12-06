using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Surreal.Text.Lexing;
using Surreal.Text.Parsing;
using static Surreal.Graphics.Materials.Shaders.ShaderStatement;

namespace Surreal.Graphics.Materials.Shaders {
  public sealed class ShaderParser : DescentParser<ShaderParser.TokenType> {
    private static readonly RegexLexer<Token> Lexer = new(
        Rule(@"\s+", lexeme => (TokenType.WhiteSpace, null), disregard: true),
        Rule(@"\+|\-|\*|\/", lexeme => (TokenType.Operator, null)),
        Rule(@"\d+", lexeme => (TokenType.Number, float.Parse(lexeme))),
        Rule(@"'[A-Za-z0-9]'", lexeme => (TokenType.String, null))
    );

    public static async Task<ShaderParser> ParseAsync(TextReader reader) {
      return new(await Lexer.TokenizeAsync(reader));
    }

    private ShaderParser(IEnumerable<Token> tokens)
        : base(tokens) {
    }

    public MetadataDeclarationStatement MetadataDeclaration() {
      throw new NotImplementedException();
    }

    public IEnumerable<ShaderDeclarationStatement> ShaderDeclarations() {
      throw new NotImplementedException();
    }

    public IEnumerable<UniformDeclarationStatement> UniformDeclarations() {
      throw new NotImplementedException();
    }

    public enum TokenType {
      WhiteSpace,
      Operator,
      Number,
      String,
    }
  }

  public abstract class ShaderStatement {
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<out T> {
      T Visit(MetadataDeclarationStatement statement);
      T Visit(ShaderDeclarationStatement statement);
      T Visit(FunctionDeclarationStatement statement);
      T Visit(UniformDeclarationStatement statement);
    }

    public enum UniformType {
      Sampler
    }

    public enum FunctionType {
      Vertex,
      Fragment
    }

    public sealed class MetadataDeclarationStatement : ShaderStatement {
      public string            Name { get; }
      public ShaderProgramType Type { get; }

      public MetadataDeclarationStatement(string name, ShaderProgramType type) {
        Name = name;
        Type = type;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class ShaderDeclarationStatement : ShaderStatement {
      public ShaderType                   Type     { get; }
      public FunctionDeclarationStatement Function { get; }

      public ShaderDeclarationStatement(ShaderType type, FunctionDeclarationStatement function) {
        Type     = type;
        Function = function;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class FunctionDeclarationStatement : ShaderStatement {
      public string                       Name       { get; }
      public FunctionType                 Type       { get; }
      public IEnumerable<ShaderStatement> Statements { get; }

      public FunctionDeclarationStatement(string name, FunctionType type, IEnumerable<ShaderStatement> statements
      ) {
        Name       = name;
        Type       = type;
        Statements = statements;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class UniformDeclarationStatement : ShaderStatement {
      public string      Name { get; }
      public UniformType Type { get; }

      public UniformDeclarationStatement(string name, UniformType type) {
        Name = name;
        Type = type;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }
  }

  public abstract class ShaderExpression {
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<out T> {
      T Visit(LiteralExpression expression);
      T Visit(UnaryExpression expression);
      T Visit(BinaryExpression expression);
    }

    public enum UnaryOperation {
      Not
    }

    public enum BinaryOperation {
      Plus,
      Minus,
      Times,
      Divide
    }

    public sealed class LiteralExpression : ShaderExpression {
      public string Name  { get; }
      public object Value { get; }

      public LiteralExpression(string name, object value) {
        Name  = name;
        Value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class UnaryExpression : ShaderExpression {
      public UnaryOperation   Operation  { get; }
      public ShaderExpression Expression { get; }

      public UnaryExpression(UnaryOperation operation, ShaderExpression expression) {
        Operation  = operation;
        Expression = expression;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class BinaryExpression : ShaderExpression {
      public BinaryOperation  Operation { get; }
      public ShaderExpression Left      { get; }
      public ShaderExpression Right     { get; }

      public BinaryExpression(BinaryOperation operation, ShaderExpression left, ShaderExpression right) {
        Operation = operation;
        Left      = left;
        Right     = right;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }
  }
}