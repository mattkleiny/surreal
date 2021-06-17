using System.Numerics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class HeadlessShaderProgram : ShaderProgram {
    public override void Bind(VertexAttributeSet attributes) {
      // no-op
    }

    public override void SetUniform(string alias, int scalar) {
      // no-op
    }

    public override void SetUniform(string alias, float scalar) {
      // no-op
    }

    public override void SetUniform(string alias, Point2 point) {
      // no-op
    }

    public override void SetUniform(string alias, Point3 point) {
      // no-op
    }

    public override void SetUniform(string alias, Vector2 vector) {
      // no-op
    }

    public override void SetUniform(string alias, Vector3 vector) {
      // no-op
    }

    public override void SetUniform(string alias, Vector4 vector) {
      // no-op
    }

    public override void SetUniform(string alias, Quaternion quaternion) {
      // no-op
    }

    public override void SetUniform(string alias, in Matrix2x2 matrix) {
      // no-op
    }

    public override void SetUniform(string alias, in Matrix3x2 matrix) {
      // no-op
    }

    public override void SetUniform(string alias, in Matrix4x4 matrix) {
      // no-op
    }
  }
}