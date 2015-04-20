using ObjectDiff.Internal;
using System;

namespace ObjectDiff
{
    /// <summary>
    /// Определитель разности объектов
    /// </summary>
    public static class Diff
    {
        /// <summary>
        /// Определение разности объектов
        /// </summary>
        /// <param name="local">Текщий объект</param>
        /// <param name="remote">Объект с новым состоянием</param>
        /// <param name="options"></param>
        /// <returns>Разность объектов</returns>
        public static DiffChain Resolve(object local, object remote, DiffOptions options = null)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }
            if (options == null)
            {
                options = DiffOptions.Default();
            }
            Type rType = remote.GetType();
            Type lType = local.GetType();
            if (!(rType == lType))
            {
                throw new Exception("Objects has different type");
            }
            var context = new ResolveContext(new ResolverMap(), lType, options);
            return context.Resolvers.FindResolver(lType).Resolve(local, remote, context);
        }
        /// <summary>
        /// Приведение объекта к новому состоянию путем применения цепочки изменений
        /// </summary>
        /// <param name="local">Текущее состояние объекта</param>
        /// <param name="differents">Цепочка изменений</param>
        /// <returns>Объект в новом состоянии</returns>
        public static object DiffMap(object local, DiffChain differents)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            foreach (var diff in differents)
            {
                var context = new MapContext(new ResolverMap(), diff, local.GetType());
                context.Resolvers.FindResolver(local.GetType()).Map(ref local, diff, context);
            }
            return local;
        }
        /// <summary>
        /// Приведение объекта к состоянию другого объекта
        /// </summary>
        /// <param name="local">Объект в текущем состоянии</param>
        /// <param name="remote">Объект с новым состоянием</param>
        /// <returns>Объект с обновленным состоянием</returns>
        public static object Map(object local, object remote)
        {
            return DiffMap(local, Resolve(local, remote));
        }
    }
}
