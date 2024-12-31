using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using newton = Newtonsoft.Json.Linq;

namespace Denomica.Text.Json
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;

    /// <summary>
    /// A converter that is used to handle converting <see cref="object"/> types when serializing and deserializing.
    /// </summary>
    internal class ObjectConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return base.CanConvert(typeToConvert);
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object? result = null;
            int originalDepth;

            switch(reader.TokenType)
            {
                case JsonTokenType.False:
                case JsonTokenType.True:
                    result = reader.GetBoolean();
                    break;

                case JsonTokenType.String:
                    result = reader.GetString();
                    break;

                case JsonTokenType.Number:
                    if(reader.TryGetInt32(out int i))
                    {
                        result = i;
                    }
                    else if(reader.TryGetDouble(out double d))
                    {
                        result = d;
                    }
                    else if(reader.TryGetDecimal(out decimal dd))
                    {
                        result = dd;
                    }
                    break;

                case JsonTokenType.StartObject:
                    var dictionary = new JsonDictionary();
                    originalDepth = reader.CurrentDepth;

                    string? key = null;

                    while (reader.Read() && reader.CurrentDepth != originalDepth && reader.TokenType != JsonTokenType.EndObject)
                    {
                        switch(reader.TokenType)
                        {
                            case JsonTokenType.PropertyName:
                                key = reader.GetString();
                                break;

                            default:
                                var obj = Read(ref reader, typeToConvert, options);
                                if(key?.Length > 0)
                                {
                                    dictionary[key] = obj;
                                }
                                key = null;
                                break;
                        }
                    }
                    result = dictionary;
                    break;

                case JsonTokenType.StartArray:
                    var list = new JsonList();
                    originalDepth = reader.CurrentDepth;

                    while(reader.Read() && reader.CurrentDepth != originalDepth && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var listItem = Read(ref reader, typeToConvert, options);
                        list.Add(listItem);
                    }
                    result = list;
                    break;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            object source = value;

            if(value is newton.JToken)
            {
                var token = (newton.JToken)value;
                source = token.ToJsonDictionary();
            }
            else if(value is JsonObject)
            {
                var obj = (JsonObject)value;
                source = obj.ToJsonDictionary();
            }
            else
            {
                source = value;
                options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            }
            //else if(value is JsonDictionary)
            //{
            //    source = value;
            //    options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            //}
            //else if(value is JsonList)
            //{
            //    source = value;
            //    options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            //}

            var json = JsonSerializer.Serialize(source, options: options);
            writer.WriteRawValue(json);
        }
    }
}
