using System.Runtime.InteropServices;
using Surreal.Colors;

namespace Surreal.Graphics.Meshes;

public sealed class VertexDescriptorTests
{
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
    [VertexDescriptor(3, VertexType.Float)]
    public Vector3 Position;

    [VertexDescriptor(4, VertexType.Float)]
    public ColorF Color;
  }
}
