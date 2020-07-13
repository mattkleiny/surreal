using System.Diagnostics.CodeAnalysis;
using Surreal.Mathematics.Tensors;
using Xunit;

namespace Surreal.Core.Mathematics {
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class TensorNDTests {
    private readonly TensorND<float> tensor = new TensorND<float>(8, 8, 8, 3);

    [Fact]
    public void it_should_evaluate_at_the_boundaries_correctly() {
      tensor[0, 0, 0, 0] = float.MaxValue;
      tensor[7, 7, 7, 2] = float.MaxValue;

      Assert.Equal(float.MaxValue, tensor[0, 0, 0, 0]);
      Assert.Equal(float.MaxValue, tensor[7, 7, 7, 2]);
    }

    [Fact]
    public void it_should_report_cardinality_correctly() {
      Assert.Equal(4, tensor.Rank);
    }

    [Fact]
    public void it_should_report_stride_correctly() {
      Assert.Equal(sizeof(float), tensor.Stride);
    }
  }
}