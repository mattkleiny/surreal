﻿namespace Surreal.Scripting.Languages.Lisp;

/// <summary>A <see cref="IScriptParser"/> for Lisp programs.</summary>
public sealed class LispScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
