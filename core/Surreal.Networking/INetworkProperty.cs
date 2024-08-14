namespace Surreal.Networking;

/// <summary>
/// A property that can be synchronized over the network.
/// </summary>
public interface INetworkProperty : IProperty
{
  NetworkAuthority Authority { get; }
}

/// <summary>
/// A property that can be synchronized over the network.
/// </summary>
public interface INetworkProperty<T> : INetworkProperty, IProperty<T>;

/// <summary>
/// The default <see cref="INetworkProperty{T}"/> implementation.
/// </summary>
public sealed class NetworkProperty<T>(T initialValue, NetworkAuthority authority = NetworkAuthority.Server) : INetworkProperty<T>
{
  public event PropertyEventHandler? PropertyChanging;
  public event PropertyEventHandler? PropertyChanged;

  public NetworkAuthority Authority => authority;

  public T Value { get; set; } = initialValue;
}
