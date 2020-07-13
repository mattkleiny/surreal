using System;

namespace Surreal.Framework.Screens {
  public interface IScreenManager : IGamePlugin {
    event Action<IScreen?> ScreenChanged;

    IScreen? ActiveScreen   { get; }
    IScreen? PreviousScreen { get; }

    void     Push(IScreen screen);
    IScreen? Pop(bool dispose = true);
  }
}