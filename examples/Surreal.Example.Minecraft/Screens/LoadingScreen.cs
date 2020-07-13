using Surreal.Framework.Screens;

namespace Minecraft.Screens {
  public static class LoadingScreens {
    public static LoadingScreen LoadAsync<TState>(this TState screen)
        where TState : GameScreen<MinecraftGame>, ILoadableScreen {
      return new LoadingScreen(screen.Game, screen);
    }
  }

  public sealed class LoadingScreen : LoadingScreen<MinecraftGame, ILoadableScreen> {
    public LoadingScreen(MinecraftGame game, ILoadableScreen screen)
        : base(game, screen) {
    }
  }
}