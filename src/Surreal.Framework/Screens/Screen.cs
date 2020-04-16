using System;
using System.Collections.Generic;
using Surreal.Timing;

namespace Surreal.Framework.Screens
{
  public abstract class Screen : IScreen
  {
    protected Screen(Game game)
    {
      Game = game;
    }

    public Game   Game  { get; }
    public IClock Clock { get; protected set; }

    public List<IScreenPlugin> Plugins { get; } = new List<IScreenPlugin>();

    public bool IsInitialized { get; private set; }
    public bool IsDisposed    { get; private set; }

    public virtual void Initialize()
    {
      IsInitialized = true;
    }

    public virtual void Show()
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Show();
      }
    }

    public virtual void Hide()
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Hide();
      }
    }

    public virtual void Begin()
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Begin();
      }
    }

    public virtual void Input(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Input(time);
      }
    }

    public virtual void Update(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Update(time);
      }
    }

    public virtual void Draw(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Draw(time);
      }
    }

    public virtual void End()
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].End();
      }
    }

    public virtual void Dispose()
    {
      foreach (var plugin in Plugins)
      {
        if (plugin is IDisposable disposable)
        {
          disposable.Dispose();
        }
      }

      IsDisposed = true;
    }
  }
}