using System.Buffers;
using Surreal.Networking.Transports;
using Surreal.Threading;

namespace Surreal.Internal.Networking;

public class SocketTransportTests
{
  [Test, Ignore("Not yet implemented")]
  public async Task it_should_communicate_between_server_and_client()
  {
    var factory = new DesktopTransportFactory();
    var options = TransportOptions.Default with
    {
      Type = TransportType.Reliability,
    };

    var semaphore = new SemaphoreSlim(0);

    await Task.WhenAll(
      ThreadFactory.Create(async () =>
      {
        await using var client = factory.CreateClientTransport(options);

        await semaphore.WaitAsync(); // wait until server is ready

        await client.ConnectToServerAsync();

        var buffer = ArrayPool<byte>.Shared.Rent(10);
        try
        {
          for (var i = 0; i < buffer.Length; i++)
          {
            buffer[i] = (byte) i;
          }

          await client.SendAsync(buffer);
        }
        finally
        {
          ArrayPool<byte>.Shared.Return(buffer);
        }
      }),
      ThreadFactory.Create(async () =>
      {
        await using var server = factory.CreateServerTransport(options);

        await server.StartServerAsync();

        semaphore.Release(); // let client proceed

        var buffer = ArrayPool<byte>.Shared.Rent(10);

        try
        {
          await server.ReceiveAsync(buffer);

          Assert.Pass("It works!");
        }
        finally
        {
          ArrayPool<byte>.Shared.Return(buffer);
        }
      })
    );

    Assert.Fail("It doesn't work!");
  }
}
