using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Graphics.Meshes;

public sealed class VertexDescriptorTests
{
  [Test]
  public void it_should_resolve_attributes_in_correct_order_from_metadata()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    attributes.Length.Should().Be(2);

    attributes[0].Alias.Should().Be("Position");
    attributes[1].Alias.Should().Be("Color");
  }

  [Test]
  public void it_should_calculate_stride_correctly()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    attributes[0].Stride.Should().Be(12);
    attributes[1].Stride.Should().Be(16);
    attributes.Stride.Should().Be(28);
  }

  [Test]
  public void it_should_calculate_offset_correctly()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    attributes[0].Offset.Should().Be(0);
    attributes[1].Offset.Should().Be(12);
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct Vertex
  {
    [VertexDescriptor(
      Count = 3,
      Type = VertexType.Float
    )]
    public Vector3 Position;

    [VertexDescriptor(
      Count = 4,
      Type = VertexType.Float,
      ShouldNormalize = true
    )]
    public Color Color;
  }
}
