using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics.Resources;

internal sealed class OpenTKShaderProgram : ShaderProgram, IHasNativeId
{
	private readonly Dictionary<string, int> locationCache = new();

	private readonly int id = GL.CreateProgram();

	int IHasNativeId.Id => id;

	public OpenTKShaderProgram(IReadOnlyList<OpenTKShader> shaders)
	{
		Link(shaders);
	}

	public void Bind()
	{
		GL.UseProgram(id);
	}

	public override void Bind(VertexDescriptorSet descriptors)
	{
		GL.UseProgram(id);

		for (var i = 0; i < descriptors.Length; i++)
		{
			var attribute = descriptors[i];
			var location = GL.GetAttribLocation(id, attribute.Alias);

			if (location == -1) continue; // attribute undefined in the shader? just move on

			GL.VertexAttribPointer(
				index: location,
				size: attribute.Count,
				type: ConvertVertexType(attribute.Type),
				normalized: attribute.Normalized,
				stride: descriptors.Stride,
				offset: attribute.Offset
			);
			GL.EnableVertexAttribArray(location);
		}
	}

	private void Link(IReadOnlyList<OpenTKShader> shaders)
	{
		var shaderIds = new int[shaders.Count];

		GL.UseProgram(id);

		for (var i = 0; i < shaders.Count; i++)
		{
			var shader = shaders[i];
			var code = shader.Code;

			var shaderId = shaderIds[i] = GL.CreateShader(shader.Type);

			GL.ShaderSource(shaderId, code);
			GL.CompileShader(shaderId);

			GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var status);

			if (status != 1)
			{
				GL.GetShaderInfoLog(shaderId, out var errorLog);

				throw new ShaderProgramException($"An error occurred whilst compiling a {shader.Type} shader.", errorLog);
			}

			GL.AttachShader(id, shaderId);
		}

		GL.LinkProgram(id);
		GL.GetProgram(id, GetProgramParameterName.LinkStatus, out var linkStatus);

		if (linkStatus != 1)
		{
			GL.GetProgramInfoLog(id, out var errorLog);

			throw new ShaderProgramException("An error occurred whilst linking a shader program.", errorLog);
		}

		foreach (var shaderId in shaderIds)
		{
			GL.DeleteShader(shaderId);
		}
	}

	public override void SetUniform(string name, int scalar)
	{
		GL.Uniform1(GetUniformLocation(name), scalar);
	}

	public override void SetUniform(string name, float scalar)
	{
		GL.Uniform1(GetUniformLocation(name), scalar);
	}

	public override void SetUniform(string name, Vector2I point)
	{
		GL.Uniform2(GetUniformLocation(name), point.X, point.Y);
	}

	public override void SetUniform(string name, Vector3I point)
	{
		GL.Uniform3(GetUniformLocation(name), point.X, point.Y, point.Z);
	}

	public override void SetUniform(string name, Vector2 vector)
	{
		GL.Uniform2(GetUniformLocation(name), vector.X, vector.Y);
	}

	public override void SetUniform(string name, Vector3 vector)
	{
		GL.Uniform3(GetUniformLocation(name), vector.X, vector.Y, vector.Z);
	}

	public override void SetUniform(string name, Vector4 vector)
	{
		GL.Uniform4(GetUniformLocation(name), vector.W, vector.X, vector.Y, vector.Z);
	}

	public override void SetUniform(string name, Quaternion quaternion)
	{
		GL.Uniform4(GetUniformLocation(name), quaternion.W, quaternion.X, quaternion.Y, quaternion.Z);
	}

	public override unsafe void SetUniform(string name, in Matrix3x2 matrix)
	{
		var location = GetUniformLocation(name);

		ref var source = ref Unsafe.AsRef(in matrix);
		var elements = (float*) Unsafe.AsPointer(ref source);

		GL.UniformMatrix4(location, 1, false, elements);
	}

	public override unsafe void SetUniform(string name, in Matrix4x4 matrix)
	{
		var location = GetUniformLocation(name);

		ref var source = ref Unsafe.AsRef(in matrix);
		var elements = (float*) Unsafe.AsPointer(ref source);

		GL.UniformMatrix4(location, 1, false, elements);
	}

	protected override void Dispose(bool managed)
	{
		GL.DeleteProgram(id);

		base.Dispose(managed);
	}

	private int GetUniformLocation(string name)
	{
		if (!locationCache.TryGetValue(name, out var id))
		{
			id = locationCache[name] = GL.GetUniformLocation(this.id, name);
		}

		return id;
	}

	private static VertexAttribPointerType ConvertVertexType(VertexType type)
	{
		switch (type)
		{
			case VertexType.UnsignedByte: return VertexAttribPointerType.UnsignedByte;
			case VertexType.Byte: return VertexAttribPointerType.Byte;
			case VertexType.Short: return VertexAttribPointerType.Short;
			case VertexType.UnsignedShort: return VertexAttribPointerType.UnsignedShort;
			case VertexType.Int: return VertexAttribPointerType.Int;
			case VertexType.UnsignedInt: return VertexAttribPointerType.UnsignedInt;
			case VertexType.Float: return VertexAttribPointerType.Float;
			case VertexType.Double: return VertexAttribPointerType.Double;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}
}
