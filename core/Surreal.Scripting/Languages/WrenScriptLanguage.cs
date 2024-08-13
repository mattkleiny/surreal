namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Wren.
/// </summary>
public sealed class WrenScriptLanguage : IScriptLanguage
{
  public string Name => "Wren";

  public ImmutableHashSet<string> SupportedExtensions { get; } = ["wren"];

  public ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
