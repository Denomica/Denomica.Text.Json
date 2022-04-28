using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denomica.Text.Json
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;

    /// <summary>
    /// Utility methods and functionality for working with JSON documents.
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// The static constructor for the utility class.
        /// </summary>
        static JsonUtil()
        {
            DefaultSerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            AddSerializationConverters(DefaultSerializationOptions);
        }

        /// <summary>
        /// The default serialization options if no options are explicitly specified.
        /// </summary>
        public static JsonSerializerOptions DefaultSerializationOptions;


        /// <summary>
        /// Creates a dictionary object from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The object to convert to a dictionary.</param>
        /// <param name="options">Optional options to use when serializing. If not specified <see cref="DefaultSerializationOptions"/> are used.</param>
        public static JsonDictionary CreateDictionary(object source, JsonSerializerOptions? options = null)
        {
            var element = JsonSerializer.SerializeToElement(source, options: options ?? DefaultSerializationOptions);
            return element.ToJsonDictionary();
        }

        /// <summary>
        /// Creates a list from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The enumerable source to convert to a list.</param>
        /// <param name="options">Optional options to use when serializing. If not specified <see cref="DefaultSerializationOptions"/> are used.</param>
        public static JsonList CreateList(IEnumerable source, JsonSerializerOptions? options = null)
        {
            var element = JsonSerializer.SerializeToElement(source, options: options ?? DefaultSerializationOptions);
            return element.ToJsonList();
        }

        /// <summary>
        /// Adds all essential JSON converters to the given options object instance.
        /// </summary>
        /// <param name="options">The options to add the converters to.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="options"/> is <c>null</c>.</exception>
        public static void AddSerializationConverters(JsonSerializerOptions options)
        {
            if(null == options) throw new ArgumentNullException(nameof(options));

            foreach(var converter in GetSerializationConverters())
            {
                options.Converters.Add(converter);
            }
        }

        /// <summary>
        /// Returns essential JSON converters for working with JSON documents, dictionaries and lists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you want to specify your own serialization options, you must make sure that you add all
        /// converters returned by this method to your custom serialization options.
        /// </para>
        /// <para>
        /// You can also use the <see cref="AddSerializationConverters"/> method to add the converters to your serialization options.
        /// </para>
        /// </remarks>
        public static IEnumerable<JsonConverter> GetSerializationConverters()
        {
            yield return new ObjectConverter();
        }
    }
}
