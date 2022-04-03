namespace Surreal.Attributes;

public class AttributeTests
{
  [Test]
  public void it_should_work()
  {
    var character = new Character();

    character.Vigor.BaseValue += 10;
  }

  private sealed record Character
  {
    public Attribute<int> Vigor     { get; } = new(AttributeType.Vigor, 10);
    public Attribute<int> Mind      { get; } = new(AttributeType.Mind, 10);
    public Attribute<int> Endurance { get; } = new(AttributeType.Endurance, 10);
  }

  private static class AttributeType
  {
    public static AttributeType<int> Vigor     { get; } = new(nameof(Vigor));
    public static AttributeType<int> Mind      { get; } = new(nameof(Mind));
    public static AttributeType<int> Endurance { get; } = new(nameof(Endurance));
    public static AttributeType<int> Strength  { get; } = new(nameof(Strength));
    public static AttributeType<int> Dexterity { get; } = new(nameof(Dexterity));
    public static AttributeType<int> Faith     { get; } = new(nameof(Faith));
    public static AttributeType<int> Luck      { get; } = new(nameof(Luck));
  }
}
