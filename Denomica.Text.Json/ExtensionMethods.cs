using System;
using System.Collections.Generic;
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

    }
}
