using Surreal.Collections;

namespace Surreal.Thinking;

/// <summary>Context for thinking operations.</summary>
public readonly struct ThinkingContext
{
	public IBlackboard Blackboard { get; }
	public object UserData { get; }
	public LevelOfDetail LevelOfDetail { get; }

	public ThinkingContext(IBlackboard blackboard, object userData, LevelOfDetail levelOfDetail)
	{
		Blackboard = blackboard;
		UserData = userData;
		LevelOfDetail = levelOfDetail;
	}
}
