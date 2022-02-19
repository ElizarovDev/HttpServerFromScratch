using System.Net.Http;

namespace Server.Itself.Handlers
{
    internal static class RequestParser
    {
        public static Request Parse(string header)
        {
            var split = header.Split(" ");
            return new Request(split[1].Trim('/'), GetMethod(split[0]));
        }

        private static HttpMethod GetMethod(string methodName)
        {
            return methodName.ToLowerInvariant() switch 
            { 
                "get" => HttpMethod.Get,
                _ => HttpMethod.Get
            };
        }
    }
}
