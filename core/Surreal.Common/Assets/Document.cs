namespace Surreal.Assets;

/// <summary>A document with a name and metadata</summary>
public sealed record Document<T>(Guid Id)
{
  public string  Name        { get; init; } = Id.ToString();
  public string? Description { get; init; }
  public T?      Contents    { get; init; }
}
