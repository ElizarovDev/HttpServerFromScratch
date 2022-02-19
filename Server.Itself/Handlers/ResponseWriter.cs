using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Server.Itself.Handlers
{
    internal static class ResponseWriter
    {
        public static async Task WriteStatusToStream(Stream stream, HttpStatusCode code)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            await writer.WriteLineAsync($"HTTP/1.0 {(int)code} {code}");
            await writer.WriteLineAsync();
        }
    }
}
