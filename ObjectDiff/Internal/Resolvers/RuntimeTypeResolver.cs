using System;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class RuntimeTypeResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return (typeof(Type).IsAssignableFrom(type));
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();

            var result = new DiffChain();
            if (remote == null)
            {
                result.Add(context.CreateDiffFromCurrentPath());
            }
            else if (local == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            else if (remote != local)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            result.Lock();
            return result;
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            local = diff.Value;
        }
    }
}
