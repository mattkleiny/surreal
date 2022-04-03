namespace Surreal.Attributes;

/// <summary>A handler delegate for <see cref="Attribute{T}"/> changes.</summary>
public delegate void AttributeChangeHandler<T>(Attribute<T> attribute, T oldValue, T newValue);

/// <summary>Commonly used <see cref="AttributeType{T}"/>s.</summary>
public static class AttributeType
{
  public static AttributeType<int> Vigor     { get; } = new(nameof(Vigor));
  public static AttributeType<int> Mind      { get; } = new(nameof(Mind));
  public static AttributeType<int> Endurance { get; } = new(nameof(Endurance));
  public static AttributeType<int> Strength  { get; } = new(nameof(Strength));
  public static AttributeType<int> Dexterity { get; } = new(nameof(Dexterity));
  public static AttributeType<int> Faith     { get; } = new(nameof(Faith));
  public static AttributeType<int> Luck      { get; } = new(nameof(Luck));
}

/// <summary>Indicates a type of attribute.</summary>
public readonly record struct AttributeType<T>(string Name)
{
  public override string ToString()
  {
    return Name;
  }
}

/// <summary>Abstracts over all possible <see cref="Attribute{T}"/>s.</summary>
public interface IAttribute
{
  Type Type { get; }
}

/// <summary>A particular attribute of a particular <see cref="AttributeType"/>.</summary>
public sealed record Attribute<T>(AttributeType<T> Type, T baseValue = default!) : IAttribute
{
  private T baseValue = baseValue;

  public event AttributeChangeHandler<T>? Changed;

  public T BaseValue
  {
    get => baseValue;
    set
    {
      var oldValue = baseValue;
      baseValue = value;

      Changed?.Invoke(this, oldValue, value);
    }
  }

  public override string ToString()
  {
    return $"{Type}: {BaseValue}";
  }

  Type IAttribute.Type => typeof(T);
}

/// <summary>A transaction to be applied to a set of <see cref="Attribute{T}"/>s.</summary>
public sealed class AttributeTransaction
{
  private readonly HashSet<Delta> deltas = new();

  public void Modify(Attribute<int> attribute, int amount)
  {
    deltas.Add(new Delta(attribute, amount));
  }

  public void Commit()
  {
    foreach (var (attribute, amount) in deltas)
    {
      attribute.BaseValue += amount;
    }
  }

  private readonly record struct Delta(Attribute<int> Attribute, int Amount);
}
