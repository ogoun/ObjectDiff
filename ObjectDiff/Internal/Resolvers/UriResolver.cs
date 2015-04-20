using System;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class UriResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return TypeHelpers.IsUri(type);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();

            var remoteUri = remote as Uri;
            var localUri = local as Uri;
            var result = new DiffChain();
            if (remoteUri == null)
            {
                result.Add(context.CreateDiffFromCurrentPath());
            }
            else if (localUri == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            else if (remoteUri.OriginalString != localUri.OriginalString)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            result.Lock();
            return result;
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            local = diff.DiffType == DiffPartType.Exclude ? null : diff.Value;
        }
    }
}
