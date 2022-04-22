using NUnit.Framework;

namespace Surreal;

/// <summary>Base class for any test case for <see cref="TGame"/>.</summary>
public abstract class GameTestCase<TGame>
  where TGame : Game, new()
{
  public IGameUnderTest<TGame> GameUnderTest { get; private set; } = null!;
  public TGame                 Game          => GameUnderTest.Instance;

  [SetUp]
  public void OnSetUp()
  {
    GameUnderTest = new GameUnderTest<TGame>(CreatePlatform(), ConfigureServices);
    GameUnderTest.Initialize();
  }

  [TearDown]
  public void OnTearDown()
  {
    GameUnderTest.Dispose();
  }

  protected virtual void ConfigureServices(IServiceRegistry services)
  {
  }

  protected virtual IPlatform CreatePlatform()
  {
    return new HeadlessPlatform();
  }
}
