namespace Surreal.Graphics.Shaders.Simple;

/// <summary>A <see cref="IShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class SimpleShadingLanguage : IShaderParser
{
  public Task<IParsedShader> ParseShaderAsync(IShaderSource source, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
