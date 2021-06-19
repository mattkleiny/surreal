using System.Numerics;
using System.Runtime.InteropServices;
using Surreal.Mathematics;
using Xunit;

namespace Surreal.Graphics.Meshes
{
  public class VertexDescriptorTests
  {
    [Fact]
    public void it_should_resolve_attributes_in_order_from_metadata()
    {
      var attributes = VertexDescriptorSet.Create<Vertex>();

      Assert.Equal(2, attributes.Length);

      Assert.Equal("a_position", attributes[0].Alias);
      Assert.Equal("a_color", attributes[1].Alias);
    }

    [Fact]
    public void it_should_calculate_stride_correctly()
    {
      var attributes = VertexDescriptorSet.Create<Vertex>();

      Assert.Equal(12, attributes[0].Stride);
      Assert.Equal(4, attributes[1].Stride);
      Assert.Equal(16, attributes.Stride);
    }

    [Fact]
    public void it_should_calculate_offset_correctly()
    {
      var attributes = VertexDescriptorSet.Create<Vertex>();

      Assert.Equal(0, attributes[0].Offset);
      Assert.Equal(12, attributes[1].Offset);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex
    {
      [VertexDescriptor(
          Alias = "a_position",
          Count = 3,
          Type  = VertexType.Float
      )]
      public Vector3 Position;

      [VertexDescriptor(
          Alias      = "a_color",
          Count      = 4,
          Type       = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;
    }
  }
}