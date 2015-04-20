using System;
using System.Net;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class IPEndPointResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return TypeHelpers.IsIpEndPoint(type);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();
            var remoteIp = remote as IPEndPoint;
            var localIp = local as IPEndPoint;
            var result = new DiffChain();
            if (remoteIp == null)
            {
                result.Add(context.CreateDiffFromCurrentPath());
            }
            else if (localIp == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            else if (remoteIp.Port != localIp.Port || remoteIp.Address.ToString() != localIp.Address.ToString())
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
