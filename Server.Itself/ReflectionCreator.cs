using Server.Itself.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Server.Itself
{
    internal class ReflectionCreator: IInstanceCreator
    {
        private readonly Dictionary<Type, Func<object>> activators = new();
        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public object CreateByExpression(Type type)
        {
            if (!activators.TryGetValue(type, out var activator))
            {
                activators.Add(type, activator = CreateActivator(type));
            }
            return activator();
        }

        private Func<object> CreateActivator(Type type)
        {
            return (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(type)).Compile();
        }
    }
}
