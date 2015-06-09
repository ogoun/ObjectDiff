using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectDiff.Internal.Resolvers
{
    internal sealed class ClassResolver : IDiffResolver
    {
        public bool IsMatch(Type type)
        {
            return type.IsClass;
        }

        static Type GetType(object local, object remote)
        {
            return (local == null) ? (remote == null) ? null : remote.GetType() : local.GetType();
        }

        public DiffChain Resolve(object local, object remote, ResolveContext context)
        {
            if (local == null && remote == null)
                return DiffChain.Empty();

            IEnumerable<MemberInfo> members = TypeMemberInfo.GetAllPublicMembers(context.Type);
            var result = new DiffChain();
            if (local == null)
            {
                result.Add(context.CreateDiffFromCurrentPath(remote));
            }
            else if (remote == null)
            {
                result.Add(context.CreateDiffFromCurrentPath());
            }
            else
            {
                foreach (var member in members)
                {
                    if (IgnoreMembersLogic.IgnoredByAttribute(context.Options, member))
                        continue;
                    var fi = member as FieldInfo;
                    if (fi != null)
                    {
                        var fieldLocal = fi.GetValue(local);
                        var fieldRemote = fi.GetValue(remote);
                        var memberType = GetType(fieldLocal, fieldRemote);
                        if (memberType != null)
                        {
                            result.Add(context.Resolvers.FindResolver(memberType).Resolve(fieldLocal, fieldRemote, new ResolveContext(context, member.Name, memberType)));
                        }
                        continue;
                    }
                    var pi = member as PropertyInfo;
                    if (pi != null)
                    {
                        var propertyLocal = pi.GetValue(local);
                        var propertyRemote = pi.GetValue(remote);
                        var memberType = GetType(propertyLocal, propertyRemote);
                        if (memberType != null)
                        {
                            result.Add(context.Resolvers.FindResolver(memberType).Resolve(propertyLocal, propertyRemote, new ResolveContext(context, member.Name, memberType)));
                        }
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
                local = diff.DiffType == DiffPartType.Exclude ? TypeHelpers.CreateDefaultState(context.Type) : diff.Value;
            }
            else
            {
                var member = TypeMemberInfo.GetPublicMember(local.GetType(), context.Path.Pop() as string);
                var fi = member as FieldInfo;
                if (fi != null)
                {
                    object fieldLocal = fi.GetValue(local);
                    Type type = (fieldLocal == null) ? fi.FieldType : fieldLocal.GetType();

                    if (diff.DiffType == DiffPartType.Exclude)
                    {
                        if (context.Path.Count == 0)
                        {
                            fi.SetValue(local, TypeHelpers.CreateDefaultState(type));
                        }
                        else
                        {
                            context.Resolvers.FindResolver(type).Map(ref fieldLocal, diff, new MapContext(context, type));
                           // fi.SetValue(local, fieldLocal);
                        }
                    }
                    else
                    {
                        context.Resolvers.FindResolver(type).Map(ref fieldLocal, diff, new MapContext(context, type));
                        fi.SetValue(local, fieldLocal);
                    }
                }
                var pi = member as PropertyInfo;
                if (pi != null)
                {
                    object propertyLocal = pi.GetValue(local);
                    Type type = (propertyLocal == null) ? pi.PropertyType : propertyLocal.GetType();

                    if (diff.DiffType == DiffPartType.Exclude)
                    {
                        if (context.Path.Count == 0)
                        {
                            pi.SetValue(local, TypeHelpers.CreateDefaultState(pi.PropertyType));
                        }
                        else
                        {
                            context.Resolvers.FindResolver(type).Map(ref propertyLocal, diff, new MapContext(context, type));
                           // pi.SetValue(local, propertyLocal);
                        }
                    }
                    else
                    {
                        context.Resolvers.FindResolver(type).Map(ref propertyLocal, diff, new MapContext(context, type));
                        pi.SetValue(local, propertyLocal);
                    }
                }
            }
        }
    }
}
