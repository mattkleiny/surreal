using NUnit.Framework;
using Surreal.Platform;

namespace Surreal.Testing;

public abstract class GameTestCase<TGame>
  where TGame : Game, new()
{
  public TGame Game { get; private set; } = null!;

  [OneTimeSetUp]
  protected virtual void InitializeGame()
  {
    Game = Surreal.Game.Create<TGame>(new()
    {
      Platform = new HeadlessPlatform()
    });
  }

  [OneTimeTearDown]
  protected virtual void ShutdownGame()
  {
    Game.Dispose();
  }
}