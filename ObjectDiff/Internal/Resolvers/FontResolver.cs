using System;
using System.Drawing;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class FontResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return type == typeof(Font);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();

            var remoteFont = remote as Font;
            var localFont = local as Font;
            var result = new DiffChain();
            if (remoteFont == null)
            {
                result.Add(context.CreateDiffFromCurrentPath());
            }
            else if (localFont == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            else if (!(Equals(remoteFont, localFont)))
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
