namespace Surreal.Scripting;

/// <summary>An event that can be triggered if it's conditions pass.</summary>
public sealed record ScriptedEvent
{
  public ConditionList Conditions { get; init; } = new();
  public ActionList    Actions    { get; init; } = new();
}
