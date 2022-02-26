using Server.Itself.Handlers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Itself
{
    public class ServerHost
    {
        private readonly IHandler handler;

        public ServerHost(IHandler handler)
        {
            this.handler = handler;
        }

        public async Task Start()
        {
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();

                using var networkStream = client.GetStream();
                using var reader = new StreamReader(networkStream);

                var firstLine = await reader.ReadLineAsync();
                var request = RequestParser.Parse(firstLine);
                await handler.HandleAsync(networkStream, request);

            }
        }
    }
}
