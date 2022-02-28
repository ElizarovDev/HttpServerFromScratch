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

        public void Handle(Stream networkStream, Request request)
        {
            var filePath = Path.Combine(path, request.Path);
            if (!File.Exists(filePath))
            {
                ResponseWriter.WriteStatusToStream(networkStream, HttpStatusCode.NotFound);
            }
            else
            {
                ResponseWriter.WriteStatusToStream(networkStream, HttpStatusCode.OK);
                using var fileStream = File.OpenRead(filePath);
                fileStream.CopyTo(networkStream);
            }
        }

        public async Task HandleAsync(Stream networkStream, Request request)
        {
            var filePath = Path.Combine(path, request.Path);
            if (!File.Exists(filePath))
            {
                await ResponseWriter.WriteStatusToStreamAsync(networkStream, HttpStatusCode.NotFound);
            }
            else
            {
                await ResponseWriter.WriteStatusToStreamAsync(networkStream, HttpStatusCode.OK);
                using var fileStream = File.OpenRead(filePath);
                await fileStream.CopyToAsync(networkStream);
            }
        }
    }
}
