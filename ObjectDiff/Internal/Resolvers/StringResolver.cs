using System;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class StringResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return type == typeof(string);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();
            if (String.Compare((string)local, (string)remote, StringComparison.Ordinal) != 0)
            {
                var result = new DiffChain
                {
                    remote == null
                        ? context.CreateDiffFromCurrentPath()
                        : context.CreateDiffFromCurrentPath(remote)
                };
                result.Lock();
                return result;
            }
            return DiffChain.Empty();
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            local = diff.DiffType == DiffPartType.Exclude ? null : diff.Value;
        }
    }
}
