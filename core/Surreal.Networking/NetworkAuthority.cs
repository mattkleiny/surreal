namespace Surreal.Networking;

/// <summary>Authority flags for client/server relationships.</summary>
[Flags]
public enum NetworkAuthority
{
  None       = 0,
  Server     = 1 << 0,
  Client     = 1 << 1,
  Everything = ~0,
}
