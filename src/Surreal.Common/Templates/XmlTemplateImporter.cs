namespace Surreal.Templates;

// TODO: flesh this out

/// <summary>Imports <see cref="ITemplate{T}"/>s from XML.</summary>
public static class XmlTemplateImporter
{
	public static TTemplate ImportTemplate<TTemplate>(Stream stream)
		where TTemplate : ITemplate
	{
		throw new NotImplementedException();
	}
}
