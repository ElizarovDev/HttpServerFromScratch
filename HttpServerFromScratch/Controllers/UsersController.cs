using Server.Itself.Handlers;

namespace HttpServerFromScratch.Controllers
{
    public record User(string Name, string Surname, string Login);

    public class UsersController : IController
    {
        public User[] Index()
        {
            return new User[] 
            {
                new User("q", "w", "e"),
                new User("a", "s", "d"),
                new User("z", "x", "c")
            };
        }
    }
}
