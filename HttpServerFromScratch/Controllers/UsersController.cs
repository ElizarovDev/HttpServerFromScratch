using Server.Itself.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServerFromScratch.Controllers
{
    public record User(string Name, string Surname, string Login);

    public class UsersController : IController
    {
        public User[] Index()
        {
            Thread.Sleep(5);
            return new User[] 
            {
                new User("q", "w", "e"),
                new User("a", "s", "d"),
                new User("z", "x", "c")
            };
        }

        public async Task<User[]> IndexAsync()
        {
            await Task.Delay(5);
            return new User[]
            {
                new User("q", "w", "e"),
                new User("a", "s", "d"),
                new User("z", "x", "c")
            };
        }
    }
}
