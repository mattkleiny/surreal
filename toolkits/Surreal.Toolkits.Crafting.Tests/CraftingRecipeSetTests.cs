namespace Surreal;

public class CraftingRecipeSetTests
{
  [Test]
  public void it_should_match_on_recipe_inputs()
  {
    var recipes = new CraftingRecipeSet<ItemKind>
    {
      CraftingRecipe.Create(1, ItemKind.Torch)
        .AddInput(2, ItemKind.Stick)
        .AddInput(1, ItemKind.Coal),

      CraftingRecipe.Create(1, ItemKind.Axe)
        .AddInput(2, ItemKind.Stick)
        .AddInput(2, ItemKind.Stone),

      CraftingRecipe.Create(1, ItemKind.Sword)
        .AddInput(3, ItemKind.Stick)
        .AddInput(1, ItemKind.Stone),
    };

    var inputs = new[]
    {
      new CraftingStack<ItemKind>(2, ItemKind.Stick),
      new CraftingStack<ItemKind>(1, ItemKind.Coal),
    };

    Assert.IsTrue(recipes.TryGetRecipe(inputs, out var recipe));
    Assert.AreEqual(ItemKind.Torch, recipe!.Output.Type);
  }

  private readonly record struct ItemKind(string Name)
  {
    public static ItemKind Stick { get; } = new(nameof(Stick));
    public static ItemKind Coal { get; } = new(nameof(Coal));
    public static ItemKind Stone { get; } = new(nameof(Stone));
    public static ItemKind Torch { get; } = new(nameof(Torch));
    public static ItemKind Sword { get; } = new(nameof(Sword));
    public static ItemKind Axe { get; } = new(nameof(Axe));

    public override string ToString()
    {
      return Name;
    }
  }
}
