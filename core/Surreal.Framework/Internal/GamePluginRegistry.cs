using Surreal.Collections;

namespace Surreal.Internal;

/// <summary>The default <see cref="IGamePluginRegistry"/> implementation.</summary>
internal sealed class GamePluginRegistry : IGamePluginRegistry
{
  private readonly List<IGamePlugin> plugins = new();

  public ReadOnlySlice<IGamePlugin> ActivePlugins => plugins;

  public void Add(IGamePlugin plugin)
  {
    plugins.Add(plugin);
  }

  public void Remove(IGamePlugin plugin)
  {
    plugins.Remove(plugin);
  }

  public void Clear()
  {
    plugins.Clear();
  }
}
