using Surreal.Framework.Screens;

namespace Minecraft.Screens {
  public static class LoadingScreens {
    public static LoadingScreen LoadInBackground<TState>(this TState screen)
        where TState : GameScreen<Game>, ILoadableScreen {
      return new LoadingScreen(screen.Game, screen);
    }
  }

  public sealed class LoadingScreen : LoadingScreen<Game, ILoadableScreen> {
    public LoadingScreen(Game game, ILoadableScreen screen)
        : base(game, screen) {
    }
  }
}