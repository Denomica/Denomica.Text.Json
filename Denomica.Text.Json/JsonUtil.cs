using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Denomica.Text.Json
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;

    public static class JsonUtil
    {


        public static JsonDictionary CreateDictionary(object source, JsonSerializerOptions? options = null)
        {
            var element = JsonSerializer.SerializeToElement(source, options: options);
            return element.ToJsonDictionary();
        }

        public static JsonList CreateList(IEnumerable source, JsonSerializerOptions? options = null)
        {
            var element = JsonSerializer.SerializeToElement(source, options: options);
            return element.ToJsonList();
        }
    }
}
