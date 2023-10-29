namespace Surreal.BehaviourTrees.Tasks;

public class FixedDelayTests
{
  [Test]
  public void it_should_wait_a_fixed_amount_of_time()
  {
    var tree = new BehaviourTree(this)
    {
      Root = new FixedDelay(TimeSpan.FromSeconds(1f))
    };

    var timeStep = TimeSpan.FromSeconds(0.25f);

    Assert.AreEqual(BehaviourStatus.Running, tree.Update(timeStep));
    Assert.AreEqual(BehaviourStatus.Running, tree.Update(timeStep));
    Assert.AreEqual(BehaviourStatus.Running, tree.Update(timeStep));
    Assert.AreEqual(BehaviourStatus.Success, tree.Update(timeStep));
  }
}
