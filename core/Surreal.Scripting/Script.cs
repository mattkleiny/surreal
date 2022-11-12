using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>Manages an underlying script resource in a <see cref="IScriptServer" />.</summary>
public sealed class Script : ScriptResource
{
  private readonly IScriptServer _server;

  public Script(IScriptServer server)
  {
    _server = server;

    Handle = server.CreateScript();
  }

  public ScriptHandle Handle { get; }

  public void RegisterFunction(string name, Delegate callback)
  {
    _server.RegisterFunction(Handle, name, callback);
  }

  public void UpdateCode(string code, VirtualPath sourcePath)
  {
    _server.CompileScriptCode(Handle, code, sourcePath);
  }

  public object? Execute()
  {
    return _server.ExecuteScript(Handle);
  }

  public object? ExecuteFunction(string functionName)
  {
    return _server.ExecuteScriptFunction(Handle, functionName);
  }

  public object? ExecuteFunction(string functionName, params object[] parameters)
  {
    return _server.ExecuteScriptFunction(Handle, functionName, parameters);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _server.DeleteScript(Handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}" /> for <see cref="Script" />s.</summary>
public sealed class ScriptLoader : AssetLoader<Script>
{
  private readonly Encoding _encoding;
  private readonly string _extension;
  private readonly IScriptServer _server;

  public ScriptLoader(IScriptServer server, string extension)
    : this(server, Encoding.UTF8, extension)
  {
  }

  public ScriptLoader(IScriptServer server, Encoding encoding, string extension)
  {
    _server = server;
    _encoding = encoding;
    _extension = extension;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && context.Path.Extension == _extension;
  }

  public override async Task<Script> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var code = await context.Path.ReadAllTextAsync(_encoding, cancellationToken);
    var script = new Script(_server);

    script.UpdateCode(code, context.Path);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Script>(ReloadAsync);
    }

    return script;
  }

  private async Task<Script> ReloadAsync(AssetLoaderContext context, Script script, CancellationToken cancellationToken)
  {
    var code = await context.Path.ReadAllTextAsync(_encoding, cancellationToken);

    script.UpdateCode(code, context.Path);

    return script;
  }
}



