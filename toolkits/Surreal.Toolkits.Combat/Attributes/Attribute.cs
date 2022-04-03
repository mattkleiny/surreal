namespace Surreal.Attributes;

/// <summary>A handler delegate for <see cref="Attribute{T}"/> changes.</summary>
public delegate void AttributeChangeHandler<T>(Attribute<T> attribute, T oldValue, T newValue);

/// <summary>Abstracts over all possible <see cref="Attribute{T}"/>s.</summary>
public interface IAttribute
{
}

/// <summary>A particular attribute of a particular <see cref="AttributeType{T}"/>.</summary>
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
}
