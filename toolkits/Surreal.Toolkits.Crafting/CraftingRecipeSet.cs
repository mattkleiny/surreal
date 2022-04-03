using System.Diagnostics.CodeAnalysis;

namespace Surreal;

public interface ICraftable<TKind>
{
  TKind Kind { get; }
}

/// <summary>A set of many <see cref="CraftingRecipe{T}"/>s.</summary>
public sealed class CraftingRecipeSet<T> : HashSet<CraftingRecipe<T>>
{
  public bool TryGetRecipe(IEnumerable<CraftingStack<T>> inputs, [NotNullWhen(true)] out CraftingRecipe<T>? result)
  {
    foreach (var recipe in this)
    {
      if (recipe.Inputs.IsSubsetOf(inputs))
      {
        result = recipe;
        return true;
      }
    }

    result = default!;
    return false;
  }
}
