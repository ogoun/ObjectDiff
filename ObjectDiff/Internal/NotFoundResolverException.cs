using System;
using System.Runtime.Serialization;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Исключение, выбрасываемое при отсутствии обработчика для типа
    /// </summary>
    public class NotFoundResolverException : Exception
    {
        public NotFoundResolverException()
        { }
        public NotFoundResolverException(string message) : base(message) { }
        public NotFoundResolverException(string message, Exception innerException) : base(message, innerException) { }
        public NotFoundResolverException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
