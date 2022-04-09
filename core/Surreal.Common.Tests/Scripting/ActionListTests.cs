using Surreal.Collections;

namespace Surreal.Scripting;

public class ActionListTests
{
  [Test]
  public async Task it_should_delegate_to_child_actions()
  {
    var counter = 0;

    var list = new ActionList
    {
      Actions.Anonymous(() => counter++),
      Actions.Anonymous(() => counter++),
      Actions.Anonymous(() => counter++),
    };

    var context = new ActionContext
    {
      Owner = new object(),
      Properties = new PropertyBag(),
    };

    await list.ExecuteAsync(context);

    counter.Should().Be(3);
  }
}
