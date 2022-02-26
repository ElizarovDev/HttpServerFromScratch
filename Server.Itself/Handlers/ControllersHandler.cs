using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Itself.Handlers
{
    public class ControllersHandler : IHandler
    {
        private readonly Dictionary<string, Func<object>> _routes;
        public ControllersHandler(Assembly controllersAssembly)
        {
            _routes = controllersAssembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IController)))
                .SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Select(x => new { ControllerType = c, Method = x }))
                .ToDictionary(
                key => GetPath(key.ControllerType, key.Method),
                value => GetEndpointMethod(value.ControllerType, value.Method));
        }

        private Func<object> GetEndpointMethod(Type controllerType, MethodInfo method)
        {
            return () => method.Invoke(Activator.CreateInstance(controllerType), Array.Empty<object>());
        }

        private string GetPath(Type controllerType, MethodInfo method)
        {
            var name = controllerType.Name;
            if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
            {
                name = name.Substring(0, name.Length - "controller".Length);
            }

            if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"/{name}";
            }
            return $"/{name}/{method.Name}";
        }

        public async Task HandleAsync(Stream networkStream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
            {
                await ResponseWriter.WriteStatusToStream(networkStream, System.Net.HttpStatusCode.NotFound);
            }
            else
            {
                await ResponseWriter.WriteStatusToStream(networkStream, System.Net.HttpStatusCode.OK);
                await WriteControllerResponseAsync(func(), networkStream);
            }
        }

        private async Task WriteControllerResponseAsync(object response, Stream networkStream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(networkStream);
                await writer.WriteAsync(str);
            }
            else if(response is byte[] buffer)
            {
                await networkStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), networkStream);
            }
        }
    }
}
