namespace Surreal.Collections.Slices;

public class ReadOnlySliceTests
{
  [Test]
  public void it_should_slice_a_valid_list()
  {
    var list = new List<int>() { 0, 1, 2, 3, 4, 5 };
    var slice = list.AsReadOnlySlice(3, 3);

    slice[0].Should().Be(3);
    slice[1].Should().Be(4);
    slice[2].Should().Be(5);
  }
}
