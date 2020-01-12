using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace FlatResults
{
    internal static class TypeExtensions
    {
        public static ConcurrentDictionary<Type, PropertyInfo[]> Properties { get; set; } = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] GetCachedProperties(this Type type)
        {
            if (Properties.ContainsKey(type))
            {
                if (Properties.TryGetValue(type, out var props))
                    return props;
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Properties.TryAdd(type, properties);
            return properties;
        }

        public static bool TryGetPropertyInfo<TEntity, TValue>(this Expression<Func<TEntity, TValue>> property, out PropertyInfo propInfo)
        {
            var member = property.Body as MemberExpression;
            propInfo = member.Member as PropertyInfo;
            return propInfo != null;
        }

        public static bool IsGenericOf<T>(this Type type)
        {
            if (!type.IsGenericType) return false;
            var genericArg = type.GetGenericArguments()[0];
            return genericArg == typeof(T);
        }
    }
}
