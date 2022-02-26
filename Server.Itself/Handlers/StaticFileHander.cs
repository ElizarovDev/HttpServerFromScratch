using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Server.Itself.Handlers
{
    public class StaticFileHander : IHandler
    {
        private readonly string path;
        public StaticFileHander(string path)
        {
            this.path = path;
        }
        public async Task HandleAsync(Stream networkStream, Request request)
        {
            //using var reader = new StreamReader(networkStream);
            //using var writer = new StreamWriter(networkStream);

            var filePath = Path.Combine(path, request.Path);
            if (!File.Exists(filePath))
            {
                await ResponseWriter.WriteStatusToStream(networkStream, HttpStatusCode.NotFound);
            }
            else
            {
                await ResponseWriter.WriteStatusToStream(networkStream, HttpStatusCode.OK);
                using var fileStream = File.OpenRead(filePath);
                await fileStream.CopyToAsync(networkStream);
            }
        }
    }
}
