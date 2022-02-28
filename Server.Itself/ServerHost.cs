using Server.Itself.Handlers;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

        public void Start()
        {
            Console.WriteLine("Start");
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                try
                {
                    var client = listener.AcceptTcpClient();

                    using var networkStream = client.GetStream();
                    using var reader = new StreamReader(networkStream);

                    var firstLine = reader.ReadLine();
                    var request = RequestParser.Parse(firstLine);
                    handler.Handle(networkStream, request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        public async Task StartAsync()
        {
            Console.WriteLine("StartAsync");
            TcpListener listner = new TcpListener(IPAddress.Any, 80);
            listner.Start();

            while (true)
            {
                var client = await listner.AcceptTcpClientAsync();
                await ProcessClientAsync(client);
            }
        }

        public void Start_Multithread()
        {
            Console.WriteLine("Start_Multithread");
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();

                ProcessClient(client);
            }
        }

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    using var networkStream = client.GetStream();
                    using var reader = new StreamReader(networkStream);

                    var firstLine = reader.ReadLine();
                    var request = RequestParser.Parse(firstLine);
                    handler.Handle(networkStream, request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = await reader.ReadLineAsync();
                    var request = RequestParser.Parse(firstLine);

                    await handler.HandleAsync(stream, request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
