using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Контекст вычисления разности объектов
    /// </summary>
    internal sealed class ResolveContext
    {
        public readonly DiffOptions Options;
        /// <summary>
        /// Обработчики
        /// </summary>
        public readonly ResolverMap Resolvers;
        /// <summary>
        /// Тип текущей исследуемой части объектов
        /// </summary>
        public readonly Type Type;
        /// <summary>
        /// Текущий путь
        /// </summary>
        private readonly Stack<string> _path;

        public ResolveContext(ResolverMap resolversMap, Type type, DiffOptions options)
        {
            Options = options;
            Resolvers = resolversMap;
            _path = new Stack<string>();
            Type = type;
        }

        public ResolveContext(ResolveContext context, string memberPath, Type type)
        {
            Resolvers = context.Resolvers;
            Options = context.Options;
            _path = new Stack<string>(context._path.Reverse());
            _path.Push(memberPath);
            Type = type;
        }
        /// <summary>
        /// Создание атома разности по текущему пути, без передачи состояния (типы разности Remove, Exclude)
        /// </summary>
        /// <param name="type">тип разности</param>
        /// <returns>Атом разности</returns>
        public DiffPart CreateDiffFromCurrentPath(DiffPartType type = DiffPartType.Exclude)
        {
            return new DiffPart(_path, null, type);
        }
        /// <summary>
        /// Создание атома разности по текущему пути с передачей значения (типы разности Create, Include, Resize)
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="type">Тип разности</param>
        /// <returns>Атом разности</returns>
        public DiffPart CreateDiffFromCurrentPath(object @value, DiffPartType type = DiffPartType.Include)
        {
            return new DiffPart(_path, @value, type);
        }
    }
}
