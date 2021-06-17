using System;

namespace Surreal.Framework.Screens {
  public interface IScreen : IDisposable {
    bool IsInitialized { get; }
    bool IsDisposed    { get; }

    void Initialize();

    void Show();
    void Hide();

    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
  }
}