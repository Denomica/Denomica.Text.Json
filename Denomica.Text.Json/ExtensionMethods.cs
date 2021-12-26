using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Denomica.Text.Json
{
    /// <summary>
    /// Defines extension methods for types in the <c>System.Text.Json</c> namespace.
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Returns the value of the current JSON element.
        /// </summary>
        /// <returns>Returns an object that represents the value of the element.</returns>
        public static object? GetValue(this JsonElement element)
        {
            object? result = null;
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    result = element.GetString();
                    break;

                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int i))
                    {
                        result = i;
                    }
                    else if (element.TryGetDouble(out double d))
                    {
                        result = d;
                    }
                    else if (element.TryGetDecimal(out decimal dd))
                    {
                        result = dd;
                    }
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    result = element.GetBoolean();
                    break;

                case JsonValueKind.Object:
                    result = element.ToDictionary();
                    break;

                case JsonValueKind.Array:
                    var arr = new ValueList();
                    foreach (var item in element.EnumerateArray())
                    {
                        arr.Add(item.GetValue());
                    }
                    result = arr;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns the value with the given key as a <see cref="ValueDictionary"/> type, if a value with
        /// the given key exists and the given value is a <see cref="ValueDictionary"/> type.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueDictionary"/>.</param>
        public static ValueDictionary? GetValueDictionary(this ValueDictionary current, string key)
        {
            if (null != current && current.ContainsKey(key) && current[key] is ValueDictionary)
            {
                return (ValueDictionary?)current[key];
            }
            return null;
        }

        /// <summary>
        /// Returns the value with the given key as a <see cref="ValueList"/> type, if a value with
        /// the given key exists and the given value is a <see cref="ValueList"/> type.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueList"/>.</param>
        public static ValueList? GetValueList(this ValueDictionary current, string key)
        {
            if (null != current && current.ContainsKey(key) && current[key] is ValueList)
            {
                return (ValueList?)current[key];
            }
            return null;
        }

        public static ValueList MergeTo(this ValueList source, ValueList target)
        {
            foreach(var item in source)
            {
                if(!target.Contains(item))
                {
                    target.Add(item);
                }
            }

            return target;
        }

        /// <summary>
        /// Merges the current source dictionary to the given target dictionary.
        /// </summary>
        /// <remarks>
        /// If the same key value pair exists in both <paramref name="source"/> and <paramref name="target"/>,
        /// the value from <paramref name="source"/> will overwrite the value in <paramref name="target"/>.
        /// </remarks>
        /// <param name="source">The source dictionary to merge to <paramref name="target"/>.</param>
        /// <param name="target">The target dictionary to merge to.</param>
        /// <returns>Returns the merged dictionary.</returns>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="source"/> or <paramref name="target"/> is <c>null</c>.</exception>
        public static ValueDictionary MergeTo(this ValueDictionary source, ValueDictionary target)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if(null == target) throw new ArgumentNullException(nameof(target));

            foreach(var key in source.Keys)
            {
                ValueDictionary? sd = null, td = null;
                ValueList? sl = null, tl = null;
                if (source.TryGetValueDictionary(key, out sd) && target.TryGetValueDictionary(key, out td))
                {
                    target[key] = sd.MergeTo(td);
                }
                else if (source.TryGetValueList(key, out sl) && target.TryGetValueList(key, out tl))
                {
                    target[key] = sl.MergeTo(tl);
                }
                else
                {
                    target[key] = source[key];
                }
            }

            return target;
        }

        /// <summary>
        /// Converts the given JSON document to a dictionary.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="document"/> is <c>null</c>.</exception>
        public static ValueDictionary ToDictionary(this JsonDocument document)
        {
            if(null == document) throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToDictionary();
        }

        /// <summary>
        /// Converts the given JSON element to a dictionary.
        /// </summary>
        public static ValueDictionary ToDictionary(this JsonElement element)
        {
            var dictionary = new ValueDictionary();

            if(element.ValueKind == JsonValueKind.Object)
            {
                foreach(var p in element.EnumerateObject())
                {
                    dictionary[p.Name] = p.Value.GetValue();
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Attempts to get the specified value from the current dictionary and return it as a <see cref="ValueDictionary"/> object.
        /// </summary>
        /// <param name="current">The current dictionary.</param>
        /// <param name="key">The key in the current dictionary that is assumed to contain another dictionary.</param>
        /// <param name="dictionary">The variable that will hold a reference to the found dictionary.</param>
        public static bool TryGetValueDictionary(this ValueDictionary current, string key, out ValueDictionary dictionary)
        {
            if (current.ContainsKey(key) && current[key] is ValueDictionary)
            {
                dictionary = current[key] as ValueDictionary ?? new ValueDictionary();
                return true;
            }

            dictionary = new ValueDictionary();
            return false;
        }

        /// <summary>
        /// Attempts to get the specified value from the current dictionary and return it as a <see cref="ValueList"/> object.
        /// </summary>
        /// <param name="current">The current dictionary.</param>
        /// <param name="key">The key in the current dictionary that is assumed to contain a <see cref="ValueList"/> object.</param>
        /// <param name="list">The variable that will hold a reference to the found list.</param>
        public static bool TryGetValueList(this ValueDictionary current, string key, out ValueList list)
        {
            if (current.ContainsKey(key) && current[key] is ValueList)
            {
                list = current[key] as ValueList ?? new ValueList();
                return true;
            }

            list = new ValueList();
            return false;
        }

    }
}
