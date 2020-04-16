using System;
using Surreal.Timing;

namespace Surreal.Framework.Screens
{
  public interface IScreen : IDisposable
  {
    bool IsInitialized { get; }
    bool IsDisposed    { get; }

    IClock Clock { get; }

    void Initialize();

    void Show();
    void Hide();

    void Begin();
    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
    void End();
  }
}