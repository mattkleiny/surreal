using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Surreal.UI.Controls.Layouts
{
  // TODO: implement me

  public class Layout : Control
  {
    public Constraint Constraint { get; }

    public Layout()
      : this(Constraints.Identity)
    {
    }

    public Layout(Constraint constraint)
    {
      Constraint = constraint;
    }

    protected override Rectangle ComputeLayout()
    {
      if (Parent != null)
      {
        return Constraint(Parent.Layout);
      }

      return Constraint(new Rectangle(Vector2.Zero, new Vector2(1920f, 1080f)));
    }
  }
}