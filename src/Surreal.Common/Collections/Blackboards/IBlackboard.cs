namespace Surreal.Collections.Blackboards
{
  /// <summary>A blackboard is a dictionary of structured types.</summary>
  public interface IBlackboard
  {
    T    Get<T>(BlackboardProperty<T> property, Optional<T> defaultValue = default);
    void Set<T>(BlackboardProperty<T> property, T value);
    void Remove<T>(BlackboardProperty<T> property);
    void Clear();
  }
}
