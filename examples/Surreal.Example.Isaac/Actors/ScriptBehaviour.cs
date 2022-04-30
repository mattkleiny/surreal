using Surreal.Scripting;

namespace Isaac.Actors;

/// <summary>Adds a <see cref="Script"/> to some actor.</summary>
public sealed class ScriptBehaviour : ActorBehaviour
{
  private readonly Script script;

  public ScriptBehaviour(Script script)
  {
    this.script = script;
  }

  public override void OnUpdate(TimeDelta deltaTime)
  {
    base.OnUpdate(deltaTime);

    script.ExecuteFunction("update", Actor, deltaTime.Seconds);
  }
}
