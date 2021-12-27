namespace Surreal.Templates;

/// <summary>A <see cref="ITemplate{T}"/> that can be imported.</summary>
public interface IImportableTemplate<out T> : ITemplate<T>
{
	void OnImportTemplate(ITemplateImportContext context);
}

/// <summary>A context for <see cref="ITemplate{T}"/> imports.</summary>
public interface ITemplateImportContext
{
	T Parse<T>(string key, T defaultValue = default!);
}
