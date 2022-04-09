namespace Surreal;

/// <summary>A stack of <see cref="Count"/> x <see cref="T"/> for use in crafting/etc.</summary>
public readonly record struct CraftingStack<T>(int Count, T Type)
{
  public override string ToString()
  {
    return $"{Count} x {Type}";
  }
}
