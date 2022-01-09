namespace Surreal;

public class TacticTests
{
  [Test, AutoFixture]
  public void it_should_evaluate_conditions_and_actions(TacticCondition condition, TacticAction action)
  {
    var context = new TacticContext();

    condition.CanExecute(context).Returns(true);
    action.CanExecute(context).Returns(true);

    var strategy = new Strategy("Prioritize healing")
    {
      Tactics =
      {
        new Tactic
        {
          Conditions = { condition },
          Actions    = { action },
        },
      },
    };

    strategy.Evaluate(context);

    condition.Received(1).CanExecute(context);
    action.Received(1).CanExecute(context);
    action.Received(1).Execute(context);
  }
}
