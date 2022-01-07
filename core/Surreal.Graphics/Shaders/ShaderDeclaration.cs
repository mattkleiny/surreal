namespace Surreal.Graphics.Shaders;

/// <summary>Different archetypes of shader programs, for template expansiosn.</summary>
public enum ShaderArchetype
{
  Sprite,
}

/// <summary>Different types of shader programs.</summary>
public enum ShaderKind
{
  Vertex,
  Geometry,
  Fragment,
}

/// <summary>Represents a parsed shader program, ready for interrogation and compilation.</summary>
public sealed record ShaderProgramDeclaration(
  string FileName,
  string Description,
  ShaderArchetype Archetype,
  params ShaderDeclaration[] Shaders
);

/// <summary>Represents a parsed shader, ready for interrogation and compilation.</summary>
public sealed record ShaderDeclaration(
  ShaderKind Kind,
  ShaderInstruction.CompilationUnit CompilationUnit
);
