using System;
using System.Collections;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class CollectionResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return null != type.GetInterface("ICollection`1") || type.Name.StartsWith("ICollection`1", StringComparison.Ordinal);
        }

        #region Collection methods
        bool Contains(object set, object value)
        {
            var parameters = new[] { value };
            return (bool)set.GetType().GetMethod("Contains").Invoke(set, parameters);
        }

        void Add(object set, object value)
        {
            var parameters = new[] { value };
            set.GetType().GetMethod("Add").Invoke(set, parameters);
        }

        void Remove(object set, object value)
        {
            var parameters = new[] { value };
            set.GetType().GetMethod("Remove").Invoke(set, parameters);
        }

        IEnumerator GetEnumerator(object set)
        {
            var parameters = new object[] { };
            return (IEnumerator)set.GetType().GetMethod("GetEnumerator").Invoke(set, parameters);
        }
        #endregion

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
                IEnumerator remoteEnumerator = GetEnumerator(remote);
                IEnumerator localEnumerator = GetEnumerator(local);

                while (localEnumerator.MoveNext())
                {
                    if (!Contains(remote, localEnumerator.Current))
                    {
                        result.Add(context.CreateDiffFromCurrentPath(localEnumerator.Current, DiffPartType.Exclude));
                    }
                }
                while (remoteEnumerator.MoveNext())
                {
                    if (!Contains(local, remoteEnumerator.Current))
                    {
                        result.Add(new ResolveContext(context, "", context.Type).CreateDiffFromCurrentPath(remoteEnumerator.Current));
                    }
                }
            }
            result.Lock();
            return result;
        }

        public void Map(ref object local, DiffPart diff, MapContext context)
        {
            switch (diff.DiffType)
            {
                case DiffPartType.Create:
                    local = diff.Value;
                    break;
                case DiffPartType.Remove:
                    local = null;
                    break;
                case DiffPartType.Exclude:
                    Remove(local, diff.Value);
                    break;
                case DiffPartType.Include:
                    Add(local, diff.Value);
                    break;
            }
        }
    }
}
