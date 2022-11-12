using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

public class NestedAutomataTests
{
  [AutoTest]
  public void it_should_tick_nested_automata_and_yield_status(IAutomata automata)
  {
    var tree = new BehaviourTree(this, new NestedAutomata(automata));
    var timeStep = 0.25f.Seconds();

    automata.Tick(Arg.Any<AutomataContext>(), timeStep).Returns(AutomataStatus.Running);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Running);

    automata.Tick(Arg.Any<AutomataContext>(), timeStep).Returns(AutomataStatus.Success);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Success);

    automata.Tick(Arg.Any<AutomataContext>(), timeStep).Returns(AutomataStatus.Failure);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Failure);
  }
}


