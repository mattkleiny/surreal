using System;
using Surreal.Mathematics.Linear;

namespace Surreal.UI.Controls.Layouts {
  public delegate Rectangle Constraint(Rectangle area);

  public static class Constraints {
    public static readonly Constraint Identity = area => area;

    public static Constraint Filled(Axis axis)      => throw new NotImplementedException();
    public static Constraint Margined(float amount) => throw new NotImplementedException();
    public static Constraint Padded(float amount)   => throw new NotImplementedException();
  }
}