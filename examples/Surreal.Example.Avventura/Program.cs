using Avventura.Screens;
using Surreal.Screens;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Avventura",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    IconPath       = "resx://Avventura/Resources/icons/avventura.png"
  }
};

Game.Start(platform, game => game.ExecuteScreen<MainScreen>());
