using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;



namespace Denomica.Text.Json
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;


    /// <summary>
    /// Defines extension methods for types in the <c>System.Text.Json</c> namespace.
    /// </summary>
    public static class ExtensionMethods
    {

        internal static JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };


        /// <summary>
        /// Deserializes the current dictionary to an instance of the type specified in <typeparamref name="T"/>.
        /// </summary>
        /// <param name="source">The dictionary to deserialize.</param>
        /// <param name="options">Optional options to control how the deserialization is done.</param>
        /// <returns></returns>
        public static T Deserialize<T>(this JsonDictionary source, JsonSerializerOptions? options = null)
        {
            var json = source.Serialize(options: options);
            return JsonSerializer.Deserialize<T>(json, options: options ?? DefaultOptions) ?? throw new Exception("Cannot deserialize.");
        }
        
        /// <summary>
        /// Deserializes the current dictionary into an instance of the type specified in <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type to deserialize to.</param>
        /// <param name="options">Options to control the deserialization.</param>
        public static object? Deserialize(this JsonDictionary source, Type targetType, JsonSerializerOptions? options = null)
        {
            var json = source.Serialize(options: options);
            return JsonSerializer.Deserialize(json, targetType, options ?? DefaultOptions);
        }

        /// <summary>
        /// Deserializes the current dictionary to an instance of the type specified in <typeparamref name="T"/>.
        /// </summary>
        /// <param name="source">The dictionary to deserialize.</param>
        /// <param name="options">Optional options to control how the deserialization is done.</param>
        public static async Task<T> DeserializeAsync<T>(this JsonDictionary source, JsonSerializerOptions? options = null)
        {
            using (var strm = new MemoryStream())
            {
                await source.SerializeAsync(strm, options: options);
                strm.Position = 0;

                return await JsonSerializer.DeserializeAsync<T>(strm, options: options ?? DefaultOptions) ?? throw new Exception("Cannot deserialize");
            }
        }

        /// <summary>
        /// Deserializes the current dictionary into an instance of the type specified in <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type to deserialize to.</param>
        /// <param name="options">Options to control the deserialization.</param>
        public static async Task<object?> DeserializeAsync(this JsonDictionary source, Type targetType, JsonSerializerOptions? options = null)
        {
            using(var strm = new MemoryStream())
            {
                await source.SerializeAsync(strm, options: options ?? DefaultOptions);
                strm.Position = 0;

                return await JsonSerializer.DeserializeAsync(strm, targetType, options: options ?? DefaultOptions);
            }
        }

        /// <summary>
        /// Returns the value of the root element for the current document.
        /// </summary>
        public static object? GetValue(this JsonDocument document)
        {
            return document.RootElement.GetValue();
        }

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
                    result = element.ToJsonDictionary();
                    break;

                case JsonValueKind.Array:
                    var arr = new JsonList();
                    foreach (var item in element.EnumerateArray())
                    {
                        arr.Add(item.GetValue());
                    }
                    result = arr;
                    break;
            }

            return result;
        }

        public static object? GetValue(this JsonNode node)
        {
            var elem = JsonSerializer.SerializeToElement(node);
            return elem.GetValue();
        }

        /// <summary>
        /// Returns the value with the given key as a dictionary, if a value with
        /// the given key exists and the given value is a dictionary.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueDictionary"/>.</param>
        public static JsonDictionary? GetValueDictionary(this JsonDictionary current, string key)
        {
            if (null != current && current.ContainsKey(key) && current[key] is JsonDictionary)
            {
                return (JsonDictionary?)current[key];
            }
            return null;
        }

        /// <summary>
        /// Returns the value with the given key as a <see cref="ValueList"/> type, if a value with
        /// the given key exists and the given value is a <see cref="ValueList"/> type.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueList"/>.</param>
        public static JsonList? GetValueList(this JsonDictionary current, string key)
        {
            if (null != current && current.ContainsKey(key) && current[key] is JsonList)
            {
                return (JsonList?)current[key];
            }
            return null;
        }

        public static JsonList MergeTo(this JsonList source, JsonList target)
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
        public static JsonDictionary MergeTo(this JsonDictionary source, JsonDictionary target)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if(null == target) throw new ArgumentNullException(nameof(target));

            foreach(var key in source.Keys)
            {
                JsonDictionary? sd = null, td = null;
                JsonList? sl = null, tl = null;
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
        /// Serializes the current dictionary to a JSON string.
        /// </summary>
        public static string Serialize(this JsonDictionary source, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(source, options: options ?? DefaultOptions);
        }

        /// <summary>
        /// Serializes the current <see cref="ValueDictionary"/> to the given stream using the optional options.
        /// </summary>
        /// <param name="strm">A stream to serialize to.</param>
        /// <param name="options">Optional options to control serialization.</param>
        /// <returns></returns>
        public static async Task SerializeAsync(this JsonDictionary source, Stream strm, JsonSerializerOptions? options = null)
        {
            await JsonSerializer.SerializeAsync(strm, source, options: options ?? DefaultOptions);
        }

        /// <summary>
        /// Converts the given JSON document to a dictionary.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="document"/> is <c>null</c>.</exception>
        public static JsonDictionary ToJsonDictionary(this JsonDocument document)
        {
            if(null == document) throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToJsonDictionary();
        }

        /// <summary>
        /// Converts the given JSON element to a dictionary.
        /// </summary>
        public static JsonDictionary ToJsonDictionary(this JsonElement element)
        {
            var dictionary = new JsonDictionary();

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
        /// Converts the given JSON object to a dictionary.
        /// </summary>
        public static JsonDictionary ToJsonDictionary(this JsonObject obj)
        {
            var dictionary = new JsonDictionary();

            foreach(var p in obj)
            {
                dictionary[p.Key] = p.Value?.GetValue();
            }

            return dictionary;
        }

        /// <summary>
        /// Converts the given JSON document to a list.
        /// </summary>
        public static JsonList ToJsonList(this JsonDocument document)
        {
            return document.RootElement.ToJsonList();
        }

        /// <summary>
        /// Converts the given JSON element to a list.
        /// </summary>
        /// <remarks>
        /// Assumes that the given <paramref name="element"/> represents a JSON array. If not, an empty list is returned.
        /// </remarks>
        public static JsonList ToJsonList(this JsonElement element)
        {
            var list = new JsonList();

            if(element.ValueKind == JsonValueKind.Array)
            {
                foreach(var item in element.EnumerateArray())
                {
                    list.Add(item.GetValue());
                }
            }
            return list;
        }

        public static JsonList ToJsonList(this JsonArray array)
        {
            var list = new JsonList();

            foreach(var item in array)
            {
                list.Add(item.GetValue());
            }

            return list;
        }

        /// <summary>
        /// Converts the current dictionary to a JSON document object.
        /// </summary>
        /// <remarks>
        /// The <see cref="JsonDocument"/> object returned implements the <see cref="IDisposable"/> interface meaning that
        /// you need to handle it accordingly.
        /// </remarks>
        public static JsonDocument ToJsonDocument(this JsonDictionary source)
        {
            var json = source.Serialize();
            return JsonDocument.Parse(json);
        }

        /// <summary>
        /// Attempts to get the specified value from the current dictionary and return it as a <see cref="ValueDictionary"/> object.
        /// </summary>
        /// <param name="current">The current dictionary.</param>
        /// <param name="key">The key in the current dictionary that is assumed to contain another dictionary.</param>
        /// <param name="dictionary">The variable that will hold a reference to the found dictionary.</param>
        public static bool TryGetValueDictionary(this JsonDictionary current, string key, out JsonDictionary dictionary)
        {
            if (current.ContainsKey(key) && current[key] is JsonDictionary)
            {
                dictionary = current[key] as JsonDictionary ?? new JsonDictionary();
                return true;
            }

            dictionary = new JsonDictionary();
            return false;
        }

        /// <summary>
        /// Attempts to get the specified value from the current dictionary and return it as a <see cref="ValueList"/> object.
        /// </summary>
        /// <param name="current">The current dictionary.</param>
        /// <param name="key">The key in the current dictionary that is assumed to contain a <see cref="ValueList"/> object.</param>
        /// <param name="list">The variable that will hold a reference to the found list.</param>
        public static bool TryGetValueList(this JsonDictionary current, string key, out JsonList list)
        {
            if (current.ContainsKey(key) && current[key] is JsonList)
            {
                list = current[key] as JsonList ?? new JsonList();
                return true;
            }

            list = new JsonList();
            return false;
        }

    }
}
