using ObjectDiff.Internal.Resolvers;
using System;
using System.Collections.Generic;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Кэш определителей разности объектов
    /// </summary>
    internal sealed class ResolverMap
    {
        private static readonly List<IDiffResolver> Resolvers = new List<IDiffResolver>
        {
            new StringResolver(),
            new EnumResolver(),            
            new ArrayResolver(),
            new RuntimeTypeResolver(),
            new ListResolver(),
            new DictionaryResolver(),
            new UriResolver(),
            new StructResolver(),
            new IPEndPointResolver(),
            new FontResolver(),
            new ValueTypeResolver(),
            new CollectionResolver(),
            new ClassResolver()
        };

        private readonly Dictionary<Type, IDiffResolver> _cachee = new Dictionary<Type, IDiffResolver>();
        /// <summary>
        /// Поиск обработчика для переданного типа
        /// </summary>
        /// <param name="type">Тип объектов</param>
        /// <returns>Обработчик</returns>
        public IDiffResolver FindResolver(Type type)
        {
            if (!_cachee.ContainsKey(type))
            {
                foreach (var resolver in Resolvers)
                {
                    if (resolver.IsMatch(type))
                    {
                        _cachee.Add(type, resolver);
                        break;
                    }
                }
            }
            if (!_cachee.ContainsKey(type))
                throw new NotFoundResolverException("Not found resolver for type " + type.FullName);
            return _cachee[type];
        }
    }
}
