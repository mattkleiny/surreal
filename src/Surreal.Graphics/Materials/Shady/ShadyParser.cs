using System;
using System.Collections.Generic;
using Surreal.Languages;
using Surreal.Languages.Lexing;
using Surreal.Languages.Parsing;
using static Surreal.Graphics.Materials.Shady.ShadyStatement;

namespace Surreal.Graphics.Materials.Shady {
  internal sealed class ShadyParser : DescentParser<ShadyParser.TokenType> {
    private static readonly RegexLexer<Token> Lexer = new RegexLexer<Token>(
        new RegexLexer<Token>.Rule(@"\s+", (lexeme, position) => new Token(TokenType.WhiteSpace, position, lexeme), disregard: true),
        new RegexLexer<Token>.Rule(@"\+|\-|\*|\/", (lexeme, position) => new Token(TokenType.Operator, position, lexeme)),
        new RegexLexer<Token>.Rule(@"\d+", (lexeme, position) => new Token(TokenType.Number, position, float.Parse(lexeme))),
        new RegexLexer<Token>.Rule(@"'[A-Za-z0-9]'", (lexeme, position) => new Token(TokenType.String, position, lexeme.Trim('.')))
    );

    public ShadyParser(SourceText text)
        : base(Lexer.Tokenize(text)) {
    }

    public MetadataDecl MetadataDeclaration() {
      throw new NotImplementedException();
    }

    public IEnumerable<ShaderDecl> ShaderDeclarations() {
      throw new NotImplementedException();
    }

    public enum TokenType {
      WhiteSpace,
      Operator,
      Number,
      String,
    }
  }
}