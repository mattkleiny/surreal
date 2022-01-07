﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Surreal.Graphics.Shaders;

/// <summary>A <see cref="IShaderParser"/> that parses C# functions into shader code using Roslyn.</summary>
public sealed class RoslynShaderParser : IShaderParser
{
  public Task<ShaderProgramDeclaration> ParseShaderAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default)
  {
    var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(reader, length), CSharpParseOptions.Default, path, cancellationToken);

    // TODO: implement me

    return Task.FromResult(new ShaderProgramDeclaration(path, string.Empty, ShaderArchetype.Sprite));
  }
}
