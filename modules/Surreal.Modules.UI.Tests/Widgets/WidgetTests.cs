using Surreal.UI.Widgets;

namespace Surreal.Widgets;

public class WidgetTests
{
  [Test]
  public void it_should_have_a_decent_api()
  {
    using var canvas = new WidgetCanvas(new Application());
  }

  private sealed record Application : StatefulWidget
  {
    private int count;

    public int Count
    {
      get => count;
      set => SetState(() => count = value);
    }

    public override Widget Create()
    {
      return new Fragment
      {
        new CheckBox("Is Enabled", false),
        new Button(
          Label: $"Click Me {Count}",
          OnClick: () => Count++
        ),
        new ListView("Test")
        {
          new ListItem("Test 1"),
          new ListItem("Test 2"),
          new ListItem("Test 3"),
          new ListItem("Test 4"),
          new ListItem("Test 5"),
        }
      };
    }
  }
}
