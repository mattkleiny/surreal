namespace Surreal.Scripting;

/// <summary>Parses script code from a particular source language.</summary>
public interface IScriptParser
{
  /// <summary>Parses a script from the given <see cref="Stream"/>.</summary>
  ValueTask<ScriptDeclaration> ParseScriptAsync(string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return ParseScriptAsync(path, reader, (int) stream.Length, cancellationToken);
  }

  /// <summary>Parses a script from the given <see cref="string"/>.</summary>
  ValueTask<ScriptDeclaration> ParseScriptAsync(string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return ParseScriptAsync(path, reader, sourceCode.Length, cancellationToken);
  }

  /// <summary>Parses a script from the given <see cref="TextReader"/>.</summary>
  ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default);
}
