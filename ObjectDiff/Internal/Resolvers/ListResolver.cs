using System;
using System.Collections;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class ListResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return TypeHelpers.IsList(type);
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();
            var result = new DiffChain();
            if (local == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote, DiffPartType.Create));
            }
            else if (remote == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(DiffPartType.Remove));
            }
            else
            {
                var localEnumerator = (IList)local;
                var remoteEnumerator = (IList)remote;
                // Сравнение пересекающихся частей
                Type[] arguments = context.Type.GetGenericArguments();
                Type elementType = arguments[0];
                var compareCount = Math.Min(localEnumerator.Count, remoteEnumerator.Count);
                for (int i = 0; i < compareCount; i++)
                {
                    result.Add(context.Resolvers.FindResolver(elementType).Resolve(localEnumerator[i], remoteEnumerator[i], new ResolveContext(context, i.ToString(), elementType)));
                }
                if (localEnumerator.Count < remoteEnumerator.Count)
                {
                    for (int i = compareCount; i < remoteEnumerator.Count; i++)
                    {
                        result.Add(context.CreateDiffFromCurrentPath(remoteEnumerator[i]));
                    }
                }
                else if (localEnumerator.Count > remoteEnumerator.Count)
                {
                    result.Add(context.CreateDiffFromCurrentPath(remoteEnumerator.Count, DiffPartType.Resize));
                }
            }
            result.Lock();
            return result;
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            var list = (IList)local;
            if (context.Path.Count == 0)
            {
                switch (diff.DiffType)
                {
                    case DiffPartType.Create:
                        local = diff.Value;
                        break;
                    case DiffPartType.Remove:
                        local = null;
                        break;
                    case DiffPartType.Resize:
                        while (list.Count > (int)diff.Value)
                        {
                            list.RemoveAt(list.Count - 1);
                        }
                        break;
                    case DiffPartType.Include:
                        list.Add(diff.Value);
                        break;
                }
            }
            else
            {
                switch (diff.DiffType)
                {
                    case DiffPartType.Include:
                        int index = Convert.ToInt32(context.Path.Pop());
                        object listValue = list[index];
                        Type[] arguments = context.Type.GetGenericArguments();
                        Type elementType = arguments[0];
                        context.Resolvers.FindResolver(elementType).Map(ref listValue, diff, new MapContext(context, elementType));
                        list[index] = listValue;
                        break;
                }
            }
        }
    }
}
