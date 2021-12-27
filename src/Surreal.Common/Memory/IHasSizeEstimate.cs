namespace Surreal.Memory;

/// <summary>Indicates a type is capable of estimating it's own <see cref="Size"/>.</summary>
public interface IHasSizeEstimate
{
	Size Size { get; }
}
