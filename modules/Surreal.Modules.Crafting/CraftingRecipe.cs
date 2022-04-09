namespace Surreal;

/// <summary>Static factory for <see cref="CraftingRecipe{T}"/>s.</summary>
public abstract class CraftingRecipe
{
  public static CraftingRecipe<T>.Builder Create<T>(int count, T item)
  {
    return new(count, item);
  }
}

/// <summary>A recipe for a single item.</summary>
public sealed class CraftingRecipe<T>
{
  /// <summary>Use <see cref="Builder"/> instead.</summary>
  private CraftingRecipe()
  {
  }

  /// <summary>The inputs required for this recipe.</summary>
  public ImmutableHashSet<CraftingStack<T>> Inputs { get; init; } = ImmutableHashSet<CraftingStack<T>>.Empty;

  /// <summary>The output from this recipe.</summary>
  public CraftingStack<T> Output { get; init; } = default;

  public override string ToString()
  {
    var builder = new StringBuilder();

    builder.Append("Recipe - Inputs (");

    var hasAnyValue = false;

    foreach (var stack in Inputs)
    {
      if (hasAnyValue)
      {
        builder.Append(", ");
      }

      builder.Append(stack);
      hasAnyValue = true;
    }

    builder.Append(") - Output (").Append(Output).Append(')');

    return builder.ToString();
  }

  public sealed class Builder
  {
    private readonly ImmutableHashSet<CraftingStack<T>>.Builder inputs = ImmutableHashSet.CreateBuilder<CraftingStack<T>>();
    private readonly CraftingStack<T> output = default;

    public Builder(int count, T item)
    {
      output = new CraftingStack<T>(count, item);
    }

    public Builder AddInput(int count, T type)
    {
      inputs.Add(new CraftingStack<T>(count, type));
      return this;
    }

    public CraftingRecipe<T> Build() => new()
    {
      Inputs = inputs.ToImmutable(),
      Output = output,
    };

    public static implicit operator CraftingRecipe<T>(Builder builder) => builder.Build();
  }
}
