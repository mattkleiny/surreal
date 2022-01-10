﻿namespace Surreal.Scripting.Languages.Basic;

/// <summary>A <see cref="IScriptParser"/> for BASIC programs.</summary>
public sealed class BasicScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}