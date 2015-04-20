using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Позволяет получить поля и свойства типа
    /// </summary>
    internal static class TypeMemberInfo
    {
        /// <summary>
        /// Кэш полей и свойств для типов
        /// </summary>
        private static readonly IDictionary<Type, IEnumerable<MemberInfo>> Cachee = new Dictionary<Type, IEnumerable<MemberInfo>>();
        /// <summary>
        /// Поиск всех открытых полей и свойств
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetAllPublicMembers(Type type)
        {
            if (!Cachee.ContainsKey(type))
            {
                var typesToScan = new List<Type>();
                for (var t = type; t != null; t = t.BaseType)
                    typesToScan.Add(t);
                if (type.IsInterface)
                    typesToScan.AddRange(type.GetInterfaces());
                // Scan all types for public properties and fields
                Cachee.Add(type, typesToScan
                    .Where(x => x != null) // filter out null types (e.g. type.BaseType == null)
                    .SelectMany(x => x.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m is FieldInfo ||
                            (m is PropertyInfo && ((PropertyInfo)m).CanRead && ((PropertyInfo)m).CanWrite && !((PropertyInfo)m).GetIndexParameters().Any()))));
            }
            return Cachee[type];
        }
        /// <summary>
        /// Получение поля или свойства по имени
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MemberInfo GetPublicMember(Type type, string name)
        {
            foreach (var mi in GetAllPublicMembers(type))
            {
                if (mi.Name.Equals(name))
                    return mi;
            }
            throw new ArgumentNullException(name, "Not found member in type " + type.FullName);
        }
    }
}
