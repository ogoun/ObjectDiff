using System;
using System.Collections.Generic;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Контекст изменения объекта
    /// </summary>
    internal sealed class MapContext
    {
        /// <summary>
        /// Набор обработчиков
        /// </summary>
        public readonly ResolverMap Resolvers;
        /// <summary>
        /// Путь к изменяемой части
        /// </summary>
        public readonly Stack<string> Path;
        /// <summary>
        /// Текущий тип обрабатываемой части объекта
        /// </summary>
        public Type Type;

        public MapContext(ResolverMap resolversMap, DiffPart diff, Type type)
        {
            Resolvers = resolversMap;
            Path = new Stack<string>();
            Type = type;
            while (diff.Path.Count > 0)
            {
                Path.Push(diff.Path.Pop());
            }
        }

        public MapContext(MapContext context, Type type)
        {
            Resolvers = context.Resolvers;
            Path = context.Path;
            Type = type;
        }
    }
}
