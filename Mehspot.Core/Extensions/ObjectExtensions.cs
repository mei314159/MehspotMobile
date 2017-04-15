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
                var parameters = new Dictionary<string, List<string>>();
                ProcessObject(content, parameters);
                if (parameters.Count > 0)
                {
                    return string.Join("&", parameters.SelectMany(a => a.Value, (arg1, arg2) => new { arg1.Key, Value = arg2 }).Select(a => a.Key + "=" + a.Value));
                }
            }

            return null;
        }

        private static void ProcessObject(object content, Dictionary<string, List<string>> parameters, string parentName = null)
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
                            if (!parameters.ContainsKey(propertyName))
                            {
                                parameters.Add(propertyName, new List<string>());
                            }

                            parameters[propertyName].Add(value.ToString());
                        }
                        else if (value is Array)
                        {
                            var arr = value as Array;
                            foreach (var item in arr)
                            {
                                if (!parameters.ContainsKey(propertyName))
                                {
                                    parameters.Add(propertyName, new List<string>());
                                }

                                parameters[propertyName].Add(item.ToString());
                            }
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
