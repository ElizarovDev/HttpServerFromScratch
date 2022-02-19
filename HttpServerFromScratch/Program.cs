using Server.Itself;
using Server.Itself.Handlers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HttpServerFromScratch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ServerHost(new StaticFileHander(Path.Combine(Environment.CurrentDirectory ,"www")));

            await server.Start();
        }
    }
}
