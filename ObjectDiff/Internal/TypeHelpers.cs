using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Набор методов для работы с типами объектов
    /// </summary>
    internal static class TypeHelpers
    {
        /// <summary>
        /// True если массив
        /// </summary>
        public static bool IsArray(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsArray;
        }
        /// <summary>
        /// True если структура
        /// </summary>
        public static bool IsStruct(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsValueType && !IsSimpleType(type);
        }
        /// <summary>
        /// True если класс
        /// </summary>
        public static bool IsClass(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsClass;
        }
        /// <summary>
        /// True если URI
        /// </summary>
        public static bool IsUri(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (typeof(Uri).IsAssignableFrom(type));
        }
        /// <summary>
        /// True если хэшсет
        /// </summary>
        public static bool IsHashSet(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        /// <summary>
        /// True если строка
        /// </summary>
        public static bool IsString(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type == typeof(string);
        }
        /// <summary>
        /// True если базовый тип - date, decimal, string, или GUID
        /// </summary>
        public static bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            return type.IsPrimitive
                   || type == typeof(DateTime)
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(Guid)
                   || type == typeof(TimeSpan);
        }
        /// <summary>
        /// True если тип данных
        /// </summary>
        public static bool IsRuntimeType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (typeof(Type).IsAssignableFrom(type));
        }
        /// <summary>
        /// True если IPEndPoint
        /// </summary>
        public static bool IsIpEndPoint(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type == typeof(IPEndPoint);
        }
        /// <summary>
        /// True если DataSet
        /// </summary>
        public static bool IsDataset(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type == typeof(DataSet);
        }
        /// <summary>
        /// True если DataTable
        /// </summary>
        public static bool IsDataTable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type == typeof(DataTable);
        }
        /// <summary>
        /// True если DataRow
        /// </summary>
        public static bool IsDataRow(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type == typeof(DataRow);
        }

        public static bool IsList(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return true;
            }
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public static bool IsDictionary(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return true;
            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());
            return baseDefinitions.Any(t => t == typeof(IDictionary<,>));
        }

        public static bool IsEnumerable(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public static bool IsGenericCollection(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return true;
            }
            IEnumerable<Type> genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
            IEnumerable<Type> baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());
            var isCollectionType = baseDefinitions.Any(t => t == typeof(ICollection<>));
            return isCollectionType;
        }

        public static bool IsNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null || type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static object CreateDefaultState(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static object CreateInitialState(Type type)
        {
            if (IsString(type))
            {
                return null;
            }
            if (IsArray(type))
            {
                return Activator.CreateInstance(type, new object[] { 0 });
            }
            if (IsList(type) || (null != type.GetInterface("ICollection`1") || type.Name.StartsWith("ICollection`1", StringComparison.Ordinal)))
            {
                Type[] arguments = type.GetGenericArguments();
                Type destListType = typeof(List<>).MakeGenericType(arguments[0]);
                return Activator.CreateInstance(destListType);
            }
            if (IsDictionary(type))
            {
                Type[] arguments = type.GetGenericArguments();
                Type dictType = typeof(Dictionary<,>).MakeGenericType(arguments[0], arguments[1]);
                return Activator.CreateInstance(dictType);
            }
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
                return Activator.CreateInstance(type);
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}
