using System.IO;
using System.Threading.Tasks;

namespace Server.Itself.Handlers
{
    public interface IHandler
    {
        Task HandleAsync(Stream stream, Request request);
    }
}
