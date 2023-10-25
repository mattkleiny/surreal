using NLua;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Scripting.Lua;

/// <summary>
/// Indicates that a type or member is used by Lua.
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
public sealed class UsedByLuaAttribute : Attribute;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lua.
/// </summary>
[RegisterService(typeof(IScriptLanguage))]
public sealed class LuaLanguage : IScriptLanguage
{
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

      // run the initial script code
      lua.DoString(code);

      return new LuaContext(lua);
    }

    /// <summary>
    /// Read/write access to the global state of Lua.
    /// </summary>
    public object this[string key]
    {
      get => lua[key];
      set => lua[key] = value;
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
