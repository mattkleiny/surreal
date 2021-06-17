using System.Numerics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class HeadlessShaderProgram : ShaderProgram {
    public override void Bind(VertexDescriptorSet descriptors) {
      // no-op
    }

    public override void SetUniform(string name, int scalar) {
      // no-op
    }

    public override void SetUniform(string name, float scalar) {
      // no-op
    }

    public override void SetUniform(string name, Point2 point) {
      // no-op
    }

    public override void SetUniform(string name, Point3 point) {
      // no-op
    }

    public override void SetUniform(string name, Vector2 vector) {
      // no-op
    }

    public override void SetUniform(string name, Vector3 vector) {
      // no-op
    }

    public override void SetUniform(string name, Vector4 vector) {
      // no-op
    }

    public override void SetUniform(string name, Quaternion quaternion) {
      // no-op
    }

    public override void SetUniform(string name, in Matrix2x2 matrix) {
      // no-op
    }

    public override void SetUniform(string name, in Matrix3x2 matrix) {
      // no-op
    }

    public override void SetUniform(string name, in Matrix4x4 matrix) {
      // no-op
    }
  }
}