﻿using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>
/// Parses script code from a particular source language.</summary>
public interface IScriptParser
{
  /// <summary>
  /// The file extensions supported by the language that this parser handles.
  /// </summary>
  string[] SupportedExtensions { get; }

  /// <summary>
  /// Parses a script from the given <see cref="TextReader" />.
  /// </summary>
  ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default);
}

/// <summary>
/// Static extensions for <see cref="IScriptParser"/>s.
/// </summary>
public static class ScriptParserExtensions
{
  /// <summary>
  /// Parses a script from the given <see cref="VirtualPath" />.
  /// </summary>
  public static ValueTask<ScriptDeclaration> ParseScriptAsync(this IScriptParser parser, VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ParseScriptAsync(parser, path, Encoding.UTF8, cancellationToken);
  }

  /// <summary>
  /// Parses a script from the given <see cref="VirtualPath" />.
  /// </summary>
  public static async ValueTask<ScriptDeclaration> ParseScriptAsync(this IScriptParser parser, VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream =  path.OpenInputStream();

    return await ParseScriptAsync(parser, path.ToString(), stream, encoding, cancellationToken);
  }

  /// <summary>
  /// Parses a script from the given <see cref="Stream" />.
  /// </summary>
  public static ValueTask<ScriptDeclaration> ParseScriptAsync(this IScriptParser parser, string path, Stream stream, CancellationToken cancellationToken = default)
  {
    return ParseScriptAsync(parser, path, stream, Encoding.UTF8, cancellationToken);
  }

  /// <summary>
  /// Parses a script from the given <see cref="Stream" />.
  /// </summary>
  public static async ValueTask<ScriptDeclaration> ParseScriptAsync(this IScriptParser parser, string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return await parser.ParseScriptAsync(path, reader, cancellationToken);
  }

  /// <summary>
  /// Parses a script from the given raw code.
  /// </summary>
  public static async ValueTask<ScriptDeclaration> ParseScriptAsync(this IScriptParser parser, string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return await parser.ParseScriptAsync(path, reader, cancellationToken);
  }
}
