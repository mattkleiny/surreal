using Surreal.Collections;
using Surreal.Timing;

namespace Surreal;

/// <summary>A strategy for the core game loop.</summary>
public interface ILoopStrategy
{
  GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime);
}

/// <summary>A target for the <see cref="ILoopStrategy"/>.</summary>
public interface ILoopTarget
{
  void Begin(GameTime time);
  void Input(GameTime time);
  void Update(GameTime time);
  void Draw(GameTime time);
  void End(GameTime time);
}

/// <summary>A <see cref="ILoopStrategy"/> that calculates an average delta time over multiple frames.</summary>
public sealed class AveragingLoopStrategy : ILoopStrategy
{
  private readonly RingBuffer<TimeSpan> samples;

  public AveragingLoopStrategy(int samples = 10)
  {
    Debug.Assert(samples > 0, "samples > 0");

    this.samples = new RingBuffer<TimeSpan>(samples);
  }

  public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();

  public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
  {
    samples.Add(deltaTime);

    var averagedDeltaTime = samples.FastAverage();

    var time = new GameTime(
      DeltaTime: averagedDeltaTime,
      TotalTime: totalTime,
      IsRunningSlowly: averagedDeltaTime > TargetDeltaTime
    );

    target.Begin(time);
    target.Input(time);
    target.Update(time);
    target.Draw(time);
    target.End(time);

    return time;
  }
}

/// <summary>A <see cref="ILoopStrategy"/> with a simple direct variable step.</summary>
public sealed class VariableStepLoopStrategy : ILoopStrategy
{
  public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

  public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
  {
    var time = new GameTime(
      DeltaTime: deltaTime,
      TotalTime: totalTime,
      IsRunningSlowly: deltaTime > TargetDeltaTime
    );

    target.Begin(time);
    target.Input(time);
    target.Update(time);
    target.Draw(time);
    target.End(time);

    return time;
  }
}

/// <summary>A <see cref="ILoopStrategy"/> with a simple direct variable step for input and graphics, and a deterministic fixed step for physics.</summary>
public sealed class FixedStepLoopStrategy : ILoopStrategy
{
  private double accumulator;

  public TimeSpan Step            { get; set; } = 16.Milliseconds();
  public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

  public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
  {
    var time = new GameTime(
      DeltaTime: deltaTime,
      TotalTime: totalTime,
      IsRunningSlowly: deltaTime > TargetDeltaTime
    );

    accumulator += deltaTime.TimeSpan.TotalSeconds;

    target.Begin(time);
    target.Input(time);

    while (accumulator >= Step.TotalSeconds)
    {
      var stepTime = new GameTime(
        DeltaTime: Step,
        TotalTime: time.TotalTime,
        IsRunningSlowly: time.IsRunningSlowly
      );

      target.Update(stepTime);

      accumulator -= Step.TotalSeconds;
    }

    target.Draw(time);
    target.End(time);

    return time;
  }
}
