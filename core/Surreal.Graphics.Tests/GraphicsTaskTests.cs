namespace Surreal.Graphics;

public class GraphicsTaskTests
{
  [Test]
  public async Task it_should_support_void_sources()
  {
    var source = GraphicsTask.Create();

    _ = Task.Run(async () =>
    {
      await Task.Delay(TimeSpan.FromMilliseconds(10));

      source.SignalCompletion();
    });

    await source.Task;
    await source.Task;
    await source.Task;

    source.IsCompleted.Should().BeTrue();
  }

  [Test]
  public async Task it_should_support_typed_sources()
  {
    var source = GraphicsTask.Create<int>();

    _ = Task.Run(async () =>
    {
      await Task.Delay(TimeSpan.FromMilliseconds(10));

      source.SignalCompletion(42);
    });

    _ = await source.Task;
    _ = await source.Task;
    var result = await source.Task;

    source.IsCompleted.Should().BeTrue();
    result.Should().Be(42);
  }
}
