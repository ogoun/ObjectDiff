using System;

namespace ObjectDiff.Internal.Resolvers
{
    /// <summary>
    /// Определитель разности объектов для конкретного типа объектов
    /// </summary>
    internal interface IDiffResolver
    {
        /// <summary>
        /// Проверка, может ли определитель обработать переданный тип
        /// </summary>
        /// <param name="type">Тип объекта</param>
        /// <returns>true - если определитель может обработать тип</returns>
        bool IsMatch(Type type);
        /// <summary>
        /// Поиск отличий в объектах
        /// </summary>
        /// <param name="local">Текущий объект</param>
        /// <param name="remote">Объект с новым состоянием</param>
        /// <param name="context">Контекст обработки</param>
        /// <returns>Цепочка отличий</returns>
        DiffChain Resolve(object local, object remote, ResolveContext context);
        /// <summary>
        /// Применение изменений к объекту
        /// </summary>
        /// <param name="local">Объект</param>
        /// <param name="diff">Атом изменения</param>
        /// <param name="context">Контекст изменения</param>
        void Map(ref object local, DiffPart diff, MapContext context);
    }
}
