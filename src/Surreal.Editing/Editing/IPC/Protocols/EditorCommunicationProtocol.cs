using System;

namespace Surreal.Framework.Editing.IPC.Protocols {
  public sealed class EditorCommunicationProtocol : IDisposable {
    private readonly ICommunicationChannel channel;

    public static EditorCommunicationProtocol CreateServer(string pipeName)
      => new EditorCommunicationProtocol(PipeCommunicationChannel.CreateServer(pipeName));

    public static EditorCommunicationProtocol CreateClient(string pipeName, string serverName = ".")
      => new EditorCommunicationProtocol(PipeCommunicationChannel.CreateClient(pipeName, serverName));

    private EditorCommunicationProtocol(ICommunicationChannel channel) {
      this.channel = channel;
    }

    public void Dispose() => channel.Dispose();

    private struct Packet {
      public PacketType Type { get; }

      public Packet(PacketType type) {
        Type = type;
      }
    }

    private enum PacketType {
      ScreenChanged
    }
  }
}