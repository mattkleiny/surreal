namespace Surreal.Scripting.LSP;

/// <summary>
/// A language server for a set of <see cref="IScriptLanguage"/>s.
/// <para/>
/// The server is responsible for providing code completion, diagnostics,
/// and other language-specific features to IDEs.
/// </summary>
public sealed class LanguageServer : IDisposable, IAsyncDisposable
{
  private readonly WebApplication _app = WebApplication.CreateBuilder().Build();

  public LanguageServer(params IScriptLanguage[] languages)
  {
    _app.MapGet("/", () => "Ready");
    _app.MapGet("/languages", () => languages.Select(it => it.Name).ToArray());
  }

  /// <summary>
  /// Runs the language server.
  /// </summary>
  public async ValueTask RunAsync(CancellationToken cancellationToken = default)
  {
    await _app.RunAsync(cancellationToken);
  }

  public void Dispose()
  {
    _app.DisposeAsync();
  }

  public ValueTask DisposeAsync()
  {
    return _app.DisposeAsync();
  }
}
