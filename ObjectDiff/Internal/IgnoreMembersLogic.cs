using System.Linq;
using System.Reflection;

namespace ObjectDiff.Internal
{
    /// <summary>
    /// Проверка на исключение поля из сравнения при поиске отличий объектов
    /// </summary>
    internal static class IgnoreMembersLogic
    {
        /// <summary>
        /// Проверка, есть ли у поля аттрибут, указывающий на его исключение из сравнения
        /// </summary>
        public static bool IgnoredByAttribute(DiffOptions options, MemberInfo info)
        {
            var attributes = info.GetCustomAttributes(true);
            return attributes.Any(a => options.IgnoreMemberAttributes.Contains(a.GetType()));
        }
    }
}
