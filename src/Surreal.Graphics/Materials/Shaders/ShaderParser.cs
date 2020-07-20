using System;
using System.Collections.Generic;
using Surreal.Languages;
using Surreal.Languages.Lexing;
using Surreal.Languages.Parsing;
using static Surreal.Graphics.Materials.Shaders.ShaderStatement;

namespace Surreal.Graphics.Materials.Shaders {
  public sealed class ShaderParser : DescentParser<ShaderParser.TokenType> {
    private static readonly RegexLexer<Token> Lexer = new RegexLexer<Token>(
        new RegexLexer<Token>.Rule(@"\s+", (lexeme, position) => new Token(TokenType.WhiteSpace, position, lexeme), disregard: true),
        new RegexLexer<Token>.Rule(@"\+|\-|\*|\/", (lexeme, position) => new Token(TokenType.Operator, position, lexeme)),
        new RegexLexer<Token>.Rule(@"\d+", (lexeme, position) => new Token(TokenType.Number, position, float.Parse(lexeme))),
        new RegexLexer<Token>.Rule(@"'[A-Za-z0-9]'", (lexeme, position) => new Token(TokenType.String, position, lexeme.Trim('.')))
    );

    public ShaderParser(SourceText text)
        : base(Lexer.Tokenize(text)) {
    }

    public MetadataDeclaration MetadataDeclaration() {
      throw new NotImplementedException();
    }

    public IEnumerable<ShaderDeclaration> ShaderDeclarations() {
      throw new NotImplementedException();
    }

    public IEnumerable<UniformDeclaration> UniformDeclarations() {
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
      T Visit(MetadataDeclaration statement);
      T Visit(ShaderDeclaration statement);
      T Visit(FunctionDeclaration statement);
      T Visit(UniformDeclaration statement);
    }

    public enum UniformType {
      Sampler
    }

    public enum FunctionType {
      Vertex,
      Fragment
    }

    public sealed class MetadataDeclaration : ShaderStatement {
      public string            Name { get; }
      public ShaderProgramType Type { get; }

      public MetadataDeclaration(string name, ShaderProgramType type) {
        Name = name;
        Type = type;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class ShaderDeclaration : ShaderStatement {
      public ShaderType          Type     { get; }
      public FunctionDeclaration Function { get; }

      public ShaderDeclaration(ShaderType type, FunctionDeclaration function) {
        Type     = type;
        Function = function;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class FunctionDeclaration : ShaderStatement {
      public string                       Name       { get; }
      public FunctionType                 Type       { get; }
      public IEnumerable<ShaderStatement> Statements { get; }

      public FunctionDeclaration(string name, FunctionType type, IEnumerable<ShaderStatement> statements
      ) {
        Name       = name;
        Type       = type;
        Statements = statements;
      }

      public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class UniformDeclaration : ShaderStatement {
      public string      Name { get; }
      public UniformType Type { get; }

      public UniformDeclaration(string name, UniformType type) {
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