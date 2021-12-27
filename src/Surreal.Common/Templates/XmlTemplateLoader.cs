using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Templates;

// TODO: flesh this out

/// <summary>Loads <see cref="ITemplate{T}"/>s from XML.</summary>
public sealed class XmlTemplateLoader<TTemplate> : AssetLoader<TTemplate>
	where TTemplate : ITemplate
{
	public override async Task<TTemplate> LoadAsync(VirtualPath path, IAssetContext context)
	{
		await using var stream = await path.OpenInputStreamAsync();

		return XmlTemplateImporter.ImportTemplate<TTemplate>(stream);
	}
}
