namespace Surreal.Threading;

public class ThreadFactoryTests
{
  [Test]
  public async Task it_should_create_a_new_thread_and_yield_a_task()
  {
    await ThreadFactory.Create(() =>
    {
      Assert.Pass("It's working!");

      return Task.CompletedTask;
    });

    Assert.Fail("It's not working!");
  }

  [Test]
  public async Task it_should_create_a_new_thread_and_yield_a_task_with_a_result()
  {
    var result = await ThreadFactory.Create(() => Task.FromResult(42));

    result.Should().Be(42);
  }
}



