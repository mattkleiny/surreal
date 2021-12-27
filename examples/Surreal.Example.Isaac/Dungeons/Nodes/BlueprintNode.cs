using System.Collections;

namespace Isaac.Dungeons.Nodes;

public abstract class BlueprintNode : IEnumerable<BlueprintNode>
{
	private List<BlueprintNode> Children { get; } = new();

	public void Add(BlueprintNode node) => Children.Add(node);
	public void Remove(BlueprintNode node) => Children.Remove(node);

	protected void PlanDungeon(DungeonPlan plan)
	{
		foreach (var child in Children)
		{
			child.PlanDungeon(plan);
		}
	}

	public IEnumerator<BlueprintNode> GetEnumerator() => Children.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
