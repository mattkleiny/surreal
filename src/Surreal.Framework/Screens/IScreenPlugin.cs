using System;

namespace Surreal.Framework.Screens {
  public interface IScreenPlugin : IDisposable {
    void Show();
    void Hide();
    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
  }
}