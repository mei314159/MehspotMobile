using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mehspot.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetQueryString(this object content)
        {
            if (content != null)
            {
                var parameters = new Dictionary<string, string>();
                ProcessObject(content, parameters);
                if (parameters.Count > 0)
                {
                    return string.Join("&", parameters.Select(a => a.Key + "=" + a.Value));
                }
            }

            return null;
        }

        private static void ProcessObject(object content, Dictionary<string, string> parameters, string parentName = null)
        {
            if (content != null)
            {
                var propInfos = content.GetType().GetRuntimeProperties();
                foreach (var propInfo in propInfos)
                {
                    var propertyName = parentName != null ? parentName + "." + propInfo.Name : propInfo.Name;
                    var value = propInfo.GetValue(content, null);
                    if (value != null)
                    {
                        if (value is ValueType || value is string)
                        {
                            parameters.Add(propertyName, value.ToString());
                        }
                        else
                        {
                            ProcessObject(value, parameters, propertyName);
                        }
                    }
                }
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
