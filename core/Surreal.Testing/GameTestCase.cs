using NUnit.Framework;

namespace Surreal;

/// <summary>Base class for any test case for <see cref="TGame"/>.</summary>
public abstract class GameTestCase<TGame>
  where TGame : Game, new()
{
  public IGameUnderTest<TGame> GameUnderTest { get; } = new GameUnderTest<TGame>();
  public TGame                 Game          => GameUnderTest.Instance;

  [SetUp]
  public async Task OnSetUp()
  {
    await GameUnderTest.InitializeAsync();
  }

  [TearDown]
  public void OnTearDown()
  {
    GameUnderTest.Dispose();
  }
}
