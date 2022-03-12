using System;

namespace Server.Itself.Interfaces
{
    public interface IInstanceCreator
    {
        object Create(Type type);
    }
}
