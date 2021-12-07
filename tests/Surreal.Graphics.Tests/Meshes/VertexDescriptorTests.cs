using System.Numerics;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Surreal.Mathematics;

namespace Surreal.Graphics.Meshes;

public sealed class VertexDescriptorTests
{
  [Test]
  public void it_should_resolve_attributes_in_correct_order_from_metadata()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    Assert.AreEqual(2, attributes.Length);

    Assert.AreEqual("a_position", attributes[0].Alias);
    Assert.AreEqual("a_color", attributes[1].Alias);
  }

  [Test]
  public void it_should_calculate_stride_correctly()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    Assert.AreEqual(12, attributes[0].Stride);
    Assert.AreEqual(4, attributes[1].Stride);
    Assert.AreEqual(16, attributes.Stride);
  }

  [Test]
  public void it_should_calculate_offset_correctly()
  {
    var attributes = VertexDescriptorSet.Create<Vertex>();

    Assert.AreEqual(0, attributes[0].Offset);
    Assert.AreEqual(12, attributes[1].Offset);
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