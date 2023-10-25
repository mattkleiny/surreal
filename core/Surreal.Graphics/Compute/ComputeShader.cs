namespace Surreal.Graphics.Compute;

/// <summary>
/// A low-level compute shader that can run on the GPU.
/// </summary>
public class ComputeShader(IGraphicsBackend backend) : GraphicsAsset
{
  // public void SetUniform(string name, Variant value)
  // {
  //   backend.SetComputeShaderUniform(this, name, value);
  // }
  //
  // public Variant GetUniform(string name)
  // {
  //   return backend.GetComputeShaderUniform(this, name);
  // }
  //
  // public void Dispatch(int x, int y, int z)
  // {
  //   backend.DispatchComputeShader(this, x, y, z);
  // }
}
