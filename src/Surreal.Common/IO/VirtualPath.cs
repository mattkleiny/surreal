using System.Diagnostics.CodeAnalysis;
using Surreal.Text;

namespace Surreal.IO;

/// <summary>Represents a path in the virtual file system.</summary>
[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
public readonly record struct VirtualPath(StringSpan Scheme, StringSpan Target)
{
	private const string SchemeSeparator = "://";

	public static VirtualPath Parse(string uri)
	{
		StringSpan scheme;
		StringSpan target;

		var index = uri.IndexOf(SchemeSeparator);
		if (index > -1)
		{
			scheme = uri.AsStringSpan(0, index);
			target = uri.AsStringSpan(index + SchemeSeparator.Length);
		}
		else
		{
			scheme = "local";
			target = default;
		}

		return new VirtualPath(scheme, target);
	}

	public string Extension => Path.GetExtension(Target.Source)!;

	public override string ToString() => $"<{Scheme.ToString()}://{Target.ToString()}>";

	public static implicit operator VirtualPath(string uri) => Parse(uri);
}
