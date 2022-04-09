using Surreal.Collections;
using Surreal.Conditions;

namespace Surreal.Scripting.Conditions;

public class ConditionListTests
{
  [Test]
  public void it_should_delegate_to_child_actions()
  {
    var list = new ConditionList();
    var context = new ConditionContext
    {
      Owner = new object(),
      Properties = new PropertyBag(),
    };

    list.Evaluate(context).Should().BeTrue();

    list.Add(Surreal.Conditions.Conditions.False);

    list.Evaluate(context).Should().BeFalse();
  }
}
