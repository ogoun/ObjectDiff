using System;
using System.Reflection;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class ArrayResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return TypeHelpers.IsArray(type);
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
                var lArr = (Array)local;
                var rArr = (Array)remote;
                var compareCount = Math.Min(lArr.Length, rArr.Length);
                Type elementType = context.Type.GetElementType();
                // Сравнение пересекающейся части
                for (int i = 0; i < compareCount; i++)
                {
                    result.Add(context.Resolvers.FindResolver(elementType).Resolve(lArr.GetValue(i), rArr.GetValue(i), new ResolveContext(context, i.ToString(), elementType)));
                }
                if (lArr.Length != rArr.Length)
                {
                    result.Add(context.CreateDiffFromCurrentPath(rArr.Length, DiffPartType.Resize));
                    if (lArr.Length < rArr.Length)
                    {
                        for (int i = compareCount; i < rArr.Length; i++)
                        {
                            result.Add(new ResolveContext(context, i.ToString(), elementType).CreateDiffFromCurrentPath(rArr.GetValue(i)));
                        }
                    }
                }
            }
            result.Lock();
            return result;
        }

        static void ResizeArray(ref object array, int n)
        {
            var type = array.GetType();
            var elemType = type.GetElementType();
            var resizeMethod = typeof(Array).GetMethod("Resize", BindingFlags.Static | BindingFlags.Public);
            var properResizeMethod = resizeMethod.MakeGenericMethod(elemType);
            var parameters = new[] { array, n };
            properResizeMethod.Invoke(null, parameters);
            array = parameters[0];
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
                    case DiffPartType.Resize:
                        ResizeArray(ref local, (int)diff.Value);
                        break;
                }
            }
            else
            {
                switch (diff.DiffType)
                {
                    case DiffPartType.Include:
                        var index = int.Parse(context.Path.Pop());
                        Type elementType = context.Type.GetElementType();
                        var array = (Array)local;
                        object arrValue = array.GetValue(index);
                        context.Resolvers.FindResolver(elementType).Map(ref arrValue, diff, new MapContext(context, elementType));
                        array.SetValue(arrValue, index);
                        break;
                }
            }
        }
    }
}
