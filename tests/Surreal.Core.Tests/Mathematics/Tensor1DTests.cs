using Surreal.Mathematics.Tensors;
using Xunit;

namespace Surreal.Core.Mathematics {
  public class Tensor1DTests {
    private readonly Tensor1D<float> tensor = new Tensor1D<float>(8);

    [Fact]
    public void it_should_evaluate_at_the_boundaries_correctly() {
      tensor[0] = float.MaxValue;
      tensor[7] = float.MaxValue;

      Assert.Equal(float.MaxValue, tensor[0]);
      Assert.Equal(float.MaxValue, tensor[7]);
    }

    [Fact]
    public void it_should_report_cardinality_correctly() {
      Assert.Equal(1, tensor.Rank);
    }

    [Fact]
    public void it_should_report_stride_correctly() {
      Assert.Equal(sizeof(float), tensor.Stride);
    }
  }
}