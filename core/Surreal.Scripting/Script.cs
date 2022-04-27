using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>Manages an underlying script resource in a <see cref="IScriptServer"/>.</summary>
public sealed class Script : ScriptResource
{
  private readonly IScriptServer server;

  public Script(IScriptServer server)
  {
    this.server = server;

    Handle = server.CreateScript();
  }

  public ScriptHandle Handle { get; }

  public void UpdateCode(string code)
  {
    server.CompileScriptCode(Handle, code);
  }

  public object? Execute()
  {
    return server.ExecuteScript(Handle);
  }

  public object? ExecuteFunction(string functionName)
  {
    return server.ExecuteScriptFunction(Handle, functionName);
  }

  public object? ExecuteFunction(string functionName, params object[] parameters)
  {
    return server.ExecuteScriptFunction(Handle, functionName, parameters);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteScript(Handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Script"/>s.</summary>
public sealed class ScriptLoader : AssetLoader<Script>
{
  private readonly IScriptServer server;
  private readonly Encoding encoding;

  public ScriptLoader(IScriptServer server)
    : this(server, Encoding.UTF8)
  {
  }

  public ScriptLoader(IScriptServer server, Encoding encoding)
  {
    this.server   = server;
    this.encoding = encoding;
  }

  public override async ValueTask<Script> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var code = await context.Path.ReadAllTextAsync(encoding, cancellationToken);
    var script = new Script(server);

    script.UpdateCode(code);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Script>(ReloadAsync);
    }

    return script;
  }

  private async ValueTask<Script> ReloadAsync(AssetLoaderContext context, Script script, CancellationToken cancellationToken)
  {
    var code = await context.Path.ReadAllTextAsync(encoding, cancellationToken);

    script.UpdateCode(code);

    return script;
  }
}
