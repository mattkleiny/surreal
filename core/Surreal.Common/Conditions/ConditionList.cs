namespace Surreal.Conditions;

/// <summary>A collection <see cref="ICondition"/>s that can be operated on in bulk.</summary>
public sealed class ConditionList : IEnumerable<ICondition>, ICondition
{
  private readonly List<ICondition> conditions = new();

  public void Add(ICondition condition) => conditions.Add(condition);
  public bool Remove(ICondition condition) => conditions.Remove(condition);

  public bool Evaluate(in ConditionContext context)
  {
    foreach (var condition in conditions)
    {
      if (!condition.Evaluate(in context))
      {
        return false;
      }
    }

    return true;
  }

  public List<ICondition>.Enumerator GetEnumerator() => conditions.GetEnumerator();
  IEnumerator<ICondition> IEnumerable<ICondition>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
