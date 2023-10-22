using NLua;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Scripting.Lua;

/// <summary>
/// Indicates that a type or member is used by Lua.
/// </summary>
[MeansImplicitUse]
public sealed class UsedByLuaAttribute : Attribute;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lua.
/// </summary>
[RegisterService(typeof(IScriptLanguage))]
public sealed class LuaLanguage : IScriptLanguage, IDisposable
{
  private readonly NLua.Lua _lua;

  public LuaLanguage()
  {
    _lua = new NLua.Lua();
    _lua.LoadCLRPackage();
  }

  public string Name => "Lua";

  /// <summary>
  /// Read/write access to the global state of Lua.
  /// </summary>
  public object this[string key]
  {
    get => _lua[key];
    set => _lua[key] = value;
  }

  /// <summary>
  /// Attempts to get a <see cref="Callable"/> from a Lua function.
  /// </summary>
  public bool TryGetCallable(string name, out Callable result)
  {
    var state = _lua[name];

    if (state is not LuaFunction function)
    {
      result = default!;
      return false;
    }

    result = args =>
    {
      var arguments = args.Select(a => a.Value).ToArray();
      var results = function.Call(arguments);

      if (results.Length > 0)
      {
        return Variant.From(results[0]);
      }

      return Variant.Null;
    };

    return true;
  }

  /// <inheritdoc/>
  public bool CanLoad(VirtualPath path)
  {
    return path.Extension.EndsWith(".lua");
  }

  /// <inheritdoc/>
  public async Task<Script> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var code = await path.ReadAllTextAsync(cancellationToken);

    return new LuaScript(this, code);
  }

  /// <inheritdoc/>
  public Variant ExecuteCode(string code)
  {
    var result = _lua.DoString(code);

    return result switch
    {
      [var value] => Variant.From(value),
      _ => Variant.Null
    };
  }

  /// <inheritdoc/>
  public Variant ExecuteFile(VirtualPath path)
  {
    var text = path.ReadAllText();

    return ExecuteCode(text);
  }

  public void Dispose()
  {
    _lua.Dispose();
  }

  /// <summary>
  /// A <see cref="Script"/> type for Lua.
  /// </summary>
  private sealed class LuaScript(LuaLanguage language, string code) : Script
  {
    public override Variant Execute()
    {
      return language.ExecuteCode(code);
    }
  }
}
