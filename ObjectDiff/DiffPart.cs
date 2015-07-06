using System;
using System.Collections.Generic;

namespace ObjectDiff
{
    /// <summary>
    /// Атом разности объектов
    /// </summary>
    [Serializable]
    public class DiffPart
    {
        /// <summary>
        /// Путь к отличающейся части
        /// </summary>
        public readonly Stack<string> Path;
        /// <summary>
        /// Тип изменения
        /// </summary>
        public readonly DiffPartType DiffType;
        /// <summary>
        /// Значение
        /// </summary>
        public readonly object Value;

        public DiffPart(Stack<string> path, object @value, DiffPartType diffType)
        {
            Path = path;
            DiffType = diffType;
            Value = @value;
        }
    }
}
