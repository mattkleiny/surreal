using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

public class NestedAutomataTests
{
  [Test, AutoFixture]
  public void it_should_tick_nested_automata_and_yield_status(IAutomata automata)
  {
    var tree     = new BehaviourTree(this, new NestedAutomata(automata));
    var timeStep = 0.25f.Seconds();

    automata.Tick(timeStep).Returns(AutomataStatus.Running);
    Assert.AreEqual(BehaviourStatus.Running, tree.Update(timeStep));

    automata.Tick(timeStep).Returns(AutomataStatus.Success);
    Assert.AreEqual(BehaviourStatus.Success, tree.Update(timeStep));

    automata.Tick(timeStep).Returns(AutomataStatus.Failure);
    Assert.AreEqual(BehaviourStatus.Failure, tree.Update(timeStep));
  }
}
