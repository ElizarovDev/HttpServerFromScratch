using Server.Itself.Interfaces;
using System.Threading.Tasks;

namespace Server.Itself
{
    internal class ReflectionExtractor : ITaskResultExtractor
    {
        public object Extract(Task task)
        {
            return task.GetType().GetProperty("Result").GetValue(task);
        }
    }
}
