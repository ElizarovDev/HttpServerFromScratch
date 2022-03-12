using Server.Itself.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Server.Itself
{
    internal class ExpressionExtractor : ITaskResultExtractor
    {
        private readonly Dictionary<Type, Func<Task, object>> extractors = new();
        public object Extract(Task task)
        {
            var type = task.GetType();
            if (!type.IsGenericType)
            {
                return null;
            }

            if (!extractors.TryGetValue(type, out var extractor))
            {
                extractors.Add(type, extractor = CreateExtractor(type));
            }
            return extractor(task);
        }

        private Func<Task, object> CreateExtractor(Type taskType)
        {
            var parameter = Expression.Parameter(typeof(Task));
            return (Func<Task, object>)Expression.Lambda(typeof(Func<Task, object>),
                Expression.Convert(Expression.Property(Expression.Convert(parameter, taskType), "Result"), typeof(object)), parameter).Compile();
        }
    }
}
