using System.Runtime.InteropServices;
using Surreal.Mathematics;

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
    [VertexDescriptor(VertexType.Float, 3)]
    public Vector3 Position;

    [VertexDescriptor(VertexType.Float, 4)]
    public Color Color;
  }
}
