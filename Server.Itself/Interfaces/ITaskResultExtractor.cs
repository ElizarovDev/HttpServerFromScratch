using System.Threading.Tasks;

namespace Server.Itself.Interfaces
{
    public interface ITaskResultExtractor
    {
        object Extract(Task task);
    }
}
