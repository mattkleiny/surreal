namespace Surreal.Objects.Templates;

/// <summary>A template for objects of type <see cref="T"/>.</summary>
public interface ITemplate
{
  object Create();
}

/// <summary>A strongly-typed <see cref="ITemplate"/></summary>
public interface ITemplate<out T> : ITemplate
{
  new T Create();

  object ITemplate.Create()
  {
    return Create()!;
  }
}

/// <summary>A <see cref="ITemplate"/> that can be imported.</summary>
public interface IImportableTemplate : ITemplate
{
  void OnImportTemplate(ITemplateImportContext context);
}

/// <summary>A <see cref="ITemplate{T}"/> that can be imported.</summary>
public interface IImportableTemplate<out T> : IImportableTemplate, ITemplate<T>
{
}

/// <summary>A context for <see cref="ITemplate{T}"/> imports.</summary>
public interface ITemplateImportContext
{
  T Parse<T>(string key, T defaultValue = default!);
}
