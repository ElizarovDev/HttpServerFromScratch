using System.Net.Http;

namespace Server.Itself.Handlers
{
    public record Request(string Path, HttpMethod HttpMethod);
}
