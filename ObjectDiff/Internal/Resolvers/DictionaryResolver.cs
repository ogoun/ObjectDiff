using ObjectDiff.Internal.Serialize;
using System;
using System.Collections;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class DictionaryResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return TypeHelpers.IsDictionary(type);
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
                var remoteDict = (IDictionary)remote;
                var localDict = (IDictionary)local;

                var remoteEnumerator = remoteDict.GetEnumerator();
                var localEnumerator = localDict.GetEnumerator();

                Type[] arguments = context.Type.GetGenericArguments();
                Type valueType = arguments[1];
                // Удаление элементов, по ключам отсутствующем в конечном состоянии
                while (localEnumerator.MoveNext())
                {
                    if (!remoteDict.Contains(localEnumerator.Key))
                    {
                        result.Add(new ResolveContext(context, ObjSerializer.Serialize(localEnumerator.Key), valueType).CreateDiffFromCurrentPath());
                    }
                }
                // Определение новых элементов и сравнение пересекающихся по ключам
                while (remoteEnumerator.MoveNext())
                {
                    if (localDict.Contains(remoteEnumerator.Key))
                    {
                        var remoteValue = remoteEnumerator.Value;
                        var localValue = localDict[remoteEnumerator.Key];
                        result.Add(context.Resolvers.FindResolver(valueType).Resolve(localValue, remoteValue, new ResolveContext(context, ObjSerializer.Serialize(remoteEnumerator.Key), valueType)));
                    }
                    else
                    {
                        result.Add(new ResolveContext(context, ObjSerializer.Serialize(remoteEnumerator.Key), valueType).CreateDiffFromCurrentPath(remoteEnumerator.Value));
                    }
                }

            }
            result.Lock();
            return result;
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
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
                }
            }
            else
            {
                if (local == null)
                {
                    local = TypeHelpers.CreateInitialState(context.Type);
                }
                var localDict = (IDictionary)local;
                var key = ObjSerializer.Deserialize(context.Path.Pop());
                if (context.Path.Count == 0)
                {
                    switch (diff.DiffType)
                    {
                        case DiffPartType.Exclude:
                            if (localDict.Contains(key))
                            {
                                localDict.Remove(key);
                            }
                            break;
                        case DiffPartType.Include:
                            if (localDict.Contains(key))
                            {
                                localDict[key] = diff.Value;
                            }
                            else
                            {
                                localDict.Add(key, diff.Value);
                            }
                            break;
                    }
                }
                else
                {
                    if (localDict.Contains(key))
                    {
                        Type[] arguments = context.Type.GetGenericArguments();
                        Type valueType = arguments[1];
                        var value = localDict[key];
                        context.Resolvers.FindResolver(valueType).Map(ref value, diff, new MapContext(context, valueType));
                        localDict[key] = value;
                    }
                    else
                    {
                        // Ключ обязан быть в словаре, т.к. движение к изменяемой части не завершено
                        throw new InvalidOperationException("Not found key " + (key ?? String.Empty));
                    }
                }
            }
        }
    }
}
