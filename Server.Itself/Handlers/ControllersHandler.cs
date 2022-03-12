using Newtonsoft.Json;
using Server.Itself.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Server.Itself.Handlers
{
    public class ControllersHandler : IHandler
    {
        private readonly Dictionary<string, Func<object>> _routes;
        private readonly IInstanceCreator _creator;
        private readonly ITaskResultExtractor _extractor;
        public ControllersHandler(Assembly controllersAssembly)
        {
            _routes = controllersAssembly.GetTypes()
                .Where(x => typeof(IController).IsAssignableFrom(x))
                .SelectMany(Controller => Controller.GetMethods().Select(Method => new
                {
                    Controller,
                    Method
                })
                ).ToDictionary(
                    key => GetPath(key.Controller, key.Method),
                    value => GetEndpointMethod(value.Controller, value.Method)
                );
            _creator = new ReflectionCreator();
            _extractor = new ReflectionExtractor();
        }

        //private Func<object> GetEndpointMethod(Type controllerType, MethodInfo method)
        //{
        //    return () => method.Invoke(Activator.CreateInstance(controllerType), Array.Empty<object>());
        //}

        private Func<object> GetEndpointMethod(Type controllerType, MethodInfo method)
        {
            return () => method.Invoke(_creator.Create(controllerType), Array.Empty<object>());
        }

        private string GetPath(Type controllerType, MethodInfo method)
        {
            string name = controllerType.Name;
            if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "controller".Length);
            if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
                return "/" + name;
            return "/" + name + "/" + method.Name;
        }

        public async Task HandleAsync(Stream networkStream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
                await ResponseWriter.WriteStatusToStreamAsync(networkStream, System.Net.HttpStatusCode.NotFound);
            else
            {
                await ResponseWriter.WriteStatusToStreamAsync(networkStream, System.Net.HttpStatusCode.OK);
                await WriteControllerResponseAsync(func(), networkStream);
            }
        }

        public void Handle(Stream networkStream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
                ResponseWriter.WriteStatusToStream(networkStream, System.Net.HttpStatusCode.NotFound);
            else
            {
                ResponseWriter.WriteStatusToStream(networkStream, System.Net.HttpStatusCode.OK);
                WriteControllerReponse(func(), networkStream);
            }
        }

        private async Task WriteControllerResponseAsync(object response, Stream networkStream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(networkStream, leaveOpen: true);
                writer.Write(str);
            }
            else if (response is byte[] buffer)
            {
                networkStream.Write(buffer, 0, buffer.Length);
            }
            else if (response is Task task)
            {
                await task;
               // await WriteControllerResponseAsync(task.GetType().GetProperty("Result").GetValue(task), networkStream);
                await WriteControllerResponseAsync(_extractor.Extract(task), networkStream);
            }
            else
            {
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), networkStream);
            }
        }

        private void WriteControllerReponse(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (response is byte[] buffer)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                WriteControllerReponse(JsonConvert.SerializeObject(response), stream);
            }
        }
    }
}
