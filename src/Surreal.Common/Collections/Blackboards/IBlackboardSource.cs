namespace Surreal.Collections.Blackboards;

/// <summary>A source for a <see cref="IBlackboard"/>.</summary>
public interface IBlackboardSource
{
  IBlackboard Blackboard { get; }
}