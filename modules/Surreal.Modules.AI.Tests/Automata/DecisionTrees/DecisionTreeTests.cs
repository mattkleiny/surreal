using Surreal.Automata.DecisionTrees;

namespace Surreal;

public class DecisionTreeTests
{
  [Test, AutoFixture]
  public async Task it_should_evaluate_conditions_and_actions(Decision.Condition condition, Decision.Action action)
  {
    var context = new DecisionContext();

    condition.CanExecute(context).Returns(true);
    action.CanExecute(context).Returns(true);

    var strategy = new DecisionTree
    {
      Decisions =
      {
        new Decision
        {
          Conditions = { condition },
          Actions    = { action },
        },
      },
    };

    await strategy.EvaluateAsync(context);

    condition.Received(1).CanExecute(context);
    action.Received(1).CanExecute(context);

    await action.Received(1).Execute(context);
  }
}
