using System;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class StructResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive && !TypeHelpers.IsSimpleType(type);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            return new ClassResolver().Resolve(local, remote, context);
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            new ClassResolver().Map(ref local, diff, context);
        }
    }
}
