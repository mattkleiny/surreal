using NLua;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Scripting.Lua;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lua.
/// </summary>
[RegisterService(typeof(IScriptLanguage))]
internal sealed class LuaLanguage : IScriptLanguage
{
  private static readonly ILog Log = LogFactory.GetLog<LuaLanguage>();

  /// <inheritdoc/>
  public bool CanLoad(VirtualPath path)
  {
    return path.Extension.EndsWith(".lua");
  }

  /// <inheritdoc/>
  public async Task<Script> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return new LuaScript(await path.ReadAllTextAsync(cancellationToken));
  }

  /// <summary>
  /// Attempts to get a <see cref="Callable"/> from a Lua function.
  /// </summary>
  private sealed class LuaContext(NLua.Lua lua) : IDisposable
  {
    private readonly Dictionary<string, Callable> _callableCache = new();

    /// <summary>
    /// Creates a new <see cref="LuaContext"/> from the given chunk of code.
    /// </summary>
    public static LuaContext CreateFromChunk(string code)
    {
      var lua = new NLua.Lua();

      lua.LoadCLRPackage();

      // import default packages
      lua.DoString("import ('Surreal', 'Surreal.Colors')");
      lua.DoString("import ('Surreal', 'Surreal.Maths')");
      lua.DoString("import ('Surreal', 'Surreal.Memory')");
      lua.DoString("import ('Surreal', 'Surreal.Text')");
      lua.DoString("import ('System.Numerics.Vectors', 'System.Numerics')");

      // replace default functions
      lua["print"] = (string message) => Log.Debug(message);

      // run the initial script code
      lua.DoString(code);

      return new LuaContext(lua);
    }

    /// <summary>
    /// Read/write access to the global state of Lua.
    /// </summary>
    public Variant this[string key]
    {
      get => Variant.From(lua[key]);
      set => lua[key] = value.Value;
    }

    /// <summary>
    /// Attempts to get a <see cref="Callable"/> representing a function with the given name.
    /// </summary>
    public bool TryGetCallable(string name, out Callable result)
    {
      // cache callables as there is a bit of overhead in constructing the closure
      if (!_callableCache.TryGetValue(name, out result!))
      {
        // try and find the function in the global state
        var state = lua[name];
        if (state is not LuaFunction function)
        {
          result = default!;
          return false;
        }

        // build a callable wrapper
        result = args =>
        {
          var arguments = args.Select(a => a.Value).ToArray();
          var results = function.Call(arguments);

          if (results.Length == 0)
          {
            return Variant.Null;
          }

          if (results.Length == 1)
          {
            return Variant.From(results[0]);
          }

          return Variant.From(results);
        };

        _callableCache[name] = result;
      }

      return true;
    }

    public void Dispose()
    {
      lua.Dispose();
    }
  }

  /// <summary>
  /// A <see cref="Script"/> type for Lua.
  /// </summary>
  private sealed class LuaScript(string code) : Script
  {
    private readonly LuaContext _context = LuaContext.CreateFromChunk(code);

    public override Variant GetGlobal(string name)
    {
      return Variant.From(_context[name]);
    }

    public override void SetGlobal(string name, Variant value)
    {
      _context[name] = value;
    }

    public override Variant ExecuteFunction(string name, params Variant[] arguments)
    {
      if (!_context.TryGetCallable(name, out var callable))
      {
        throw new InvalidOperationException($"Unable to locate function {name}");
      }

      return callable(arguments);
    }

    public override void Dispose()
    {
      _context.Dispose();

      base.Dispose();
    }
  }
}
