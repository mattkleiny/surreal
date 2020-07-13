using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Avventura.Model.Plots {
  public sealed class Plot : IEnumerable<PlotPoint> {
    private readonly PlotPoint[] points;

    public Plot(params PlotPoint[] points) {
      this.points = points;
    }

    public PlotPoint this[Index index] => points[index];
    public Plot this[Range range] => new Plot(points[range]);

    public IEnumerator<PlotPoint> GetEnumerator() => points.Cast<PlotPoint>().GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
  }

  public readonly struct PlotPoint {
    public static PlotPoint Blend(PlotPoint a, PlotPoint b, float t) {
      throw new NotImplementedException();
    }
  }
}