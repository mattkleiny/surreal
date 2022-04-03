namespace Surreal.Attributes;

public class AttributeTests
{
  [Test]
  public void it_should_work()
  {
    var character = new Character();

    character.Vigor.BaseValue += 10;
  }

  [Test]
  public void it_should_transact()
  {
    var character   = new Character();
    var transaction = new AttributeTransaction();

    transaction.Modify(character.Vigor, 1);
    transaction.Modify(character.Vigor, -2);

    transaction.Commit();
  }

  public sealed record Character
  {
    public Attribute<int> Vigor     { get; } = new(AttributeType.Vigor, 10);
    public Attribute<int> Mind      { get; } = new(AttributeType.Mind, 10);
    public Attribute<int> Endurance { get; } = new(AttributeType.Endurance, 10);
  }
}
