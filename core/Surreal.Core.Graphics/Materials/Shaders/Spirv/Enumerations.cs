using System;

namespace Surreal.Graphics.Materials.Shaders.Spirv {
  internal enum PrimitiveType {
    Boolean,
    Integer,
    FloatingPoint,
    Numerical,
    Scalar,
    Vector,
    Matrix,
    Array,
    Image,
  }

  [Flags]
  internal enum Capabilities {
    None   = 0,
    Matrix = 1 << 0,
    Shader = 1 << 1,
  }

  [Flags]
  internal enum ExecutionMode {
    None               = 0,
    PixelCenterInteger = 1 << 0,
  }

  internal enum AddressingModel {
    Logical,
  }

  internal enum MemoryModel {
    Simple,
  }
}