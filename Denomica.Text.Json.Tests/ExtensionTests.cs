using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using JSON = Denomica.Text.Json;
using Newton = Newtonsoft.Json.Linq;

namespace Denomica.Text.Json.Tests
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;

    [TestClass]
    public class ExtensionTests
    {
        static ExtensionTests()
        {
            SerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            JsonUtil.AddSerializationConverters(SerializationOptions);
        }

        private static JsonSerializerOptions SerializationOptions;

        [TestMethod]
        public async Task Deserialize01()
        {
            var source = new Dictionary<string, object?>
            {
                ["endpoint"] = Newton.JObject.Parse("{ \"uri\": \"https://localhost/api/foo\" }")
            };

            object? target = null;

            using (var strm = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(strm, source, options: SerializationOptions);
                strm.Position = 0;

                target = await JsonSerializer.DeserializeAsync(strm, typeof(Dictionary<string, object?>), options: SerializationOptions);
            }

            Assert.IsNotNull(target);
        }

        [TestMethod]
        public async Task Deserialize02()
        {
            var uri = "https://localhost";
            var source = new JsonDictionary
            {
                ["endpoint"] = Newtonsoft.Json.Linq.JObject.Parse($"{{ \"uri\": \"{uri}\" }}")
            };

            var target = await source.DeserializeAsync<EndpointResponse>(options: SerializationOptions);
        }

        [TestMethod]
        public async Task Deserialize03()
        {
            var uri = "https://localhost";
            var source = new JsonDictionary
            {
                ["endpoint"] = Newton.JObject.Parse($"{{ \"uri\": \"{uri}\" }}")
            };

            // No exception must be thrown when deserializing.
            var target = await source.DeserializeAsync(typeof(EndpointResponse), options: SerializationOptions);
            Assert.IsNotNull(target);
            Assert.IsTrue(target is EndpointResponse);
        }

        [TestMethod]
        public void Deserialize04()
        {
            var source = new JsonDictionary
            {
                {
                    "records",
                    new JsonList
                    {
                        Newton.JToken.Parse("{ \"id\": \"26556e41-4177-4491-9a73-02b0af1b6313\"}"),
                        JsonNode.Parse("{ \"index\": 5, \"text\": \"some text\"}")
                    }
                }
            };

            var set = source.Deserialize<RecordSet>(options: SerializationOptions);
            Assert.IsNotNull(set);
            Assert.AreEqual(2, set.Records.Count);
            CollectionAssert.AllItemsAreInstancesOfType(set.Records, typeof(JsonDictionary));
            var dictionaries = from x in set.Records select (JsonDictionary)x;
            Assert.IsTrue(dictionaries.All(x => x.Count > 0));
        }

        [TestMethod]
        public void Deserialize05()
        {
            var jsonArr = JsonNode.Parse("[{ \"foo\": \"bar\", \"list\": [ \"Hello\", \"World!\" ] }, { \"child\": { \"grandChild\": { \"name\": \"Charles\", \"age\": 10 } }  }]");
            var list = JsonSerializer.Deserialize<JsonList>(jsonArr, options: SerializationOptions);

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            CollectionAssert.AllItemsAreInstancesOfType(list, typeof(JsonDictionary));

            var item1 = list[0] as JsonDictionary;
            var item2 = list[1] as JsonDictionary;
            Assert.IsNotNull(item1);
            Assert.IsNotNull(item2);

            Assert.AreEqual("bar", item1["foo"]);
            var subList = item1["list"] as JsonList;
            Assert.IsNotNull(subList);
            CollectionAssert.Contains(subList, "Hello");
            CollectionAssert.Contains(subList, "World!");

            var child = item2["child"] as JsonDictionary;
            Assert.IsNotNull(child);

            var grandChild = child["grandChild"] as JsonDictionary;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual("Charles", grandChild["name"]);
            Assert.AreEqual(10, grandChild["age"]);
        }

        [TestMethod]
        public async Task Deserialize06()
        {
            var source = new JsonDictionary
            {
                ["endpoint"] = new JsonDictionary { ["url"] = "https://localhost/api/foo" }
            };

            object? target = null;

            using (var strm = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(strm, source, options: SerializationOptions);
                strm.Position = 0;

                target = await JsonSerializer.DeserializeAsync(strm, typeof(JsonDictionary), options: SerializationOptions);
            }

            Assert.IsNotNull(target);
        }



        [TestMethod]
        public void GetValue01()
        {
            JsonElement? elem = null;
            var val = elem.GetValue();
            Assert.IsNull(val);
        }


        [TestMethod]
        public async Task Serialize01()
        {
            var source = new Dictionary<string, object?>();
            source["sub"] = Newton.JObject.Parse("{ \"foo\": \"bar\" }");
            var json = await source.SerializeAsync();

            var jDoc = JsonDocument.Parse(json);
            var target = jDoc.ToJsonDictionary();
            
            Assert.IsNotNull(target);
            JsonDictionary sub = (JsonDictionary)(target["sub"] ?? throw new NullReferenceException());
            Assert.AreEqual("bar", sub["foo"]);
        }



        [TestMethod]
        public void MergeList01()
        {
            var source = new List<object?> { 1, 2, 3, 4 };
            var target = new List<object?> { 5, 6, 7, 8 };

            var merged = source.MergeTo(target);
            Assert.AreEqual(8, merged.Count);
            CollectionAssert.AllItemsAreUnique(merged);
        }

        [TestMethod]
        public void MergeList02()
        {
            var source = new List<object?>
            {
                1,
                3.141592653589793238,
                new Dictionary<string, object?>
                {
                    { "id", "value" }
                },
                true
            };
            var target = new List<object?>
            {
                true,
                new Dictionary<string, object?>
                {
                    { "id", "value" }
                }
            };

            var merged = source.MergeTo(target);
            Assert.AreEqual(5, merged.Count);

            var dictionaries = from x in merged where x is Dictionary<string, object?> select x;
            Assert.AreEqual(2, dictionaries.Count());

            CollectionAssert.Contains(merged, 1);
            CollectionAssert.Contains(merged, true);
            CollectionAssert.Contains(merged, 3.141592653589793238);
        }



        [TestMethod]
        public void MergeDictionary01()
        {
            var source = JsonDocument.Parse(Properties.Resources.json02a).ToJsonDictionary();
            var target = JsonDocument.Parse(Properties.Resources.json02).ToJsonDictionary();

            var merged = source.MergeTo(target);
            var menu = merged.GetValueDictionary("menu");

            Assert.IsNotNull(menu);
            Assert.AreEqual("file-a", menu["id"]);
        }

        [TestMethod]
        public void MergeDictionary02()
        {
            var source = JsonDocument.Parse("{ \"menu\": { \"header\": \"Super Viewer\" }, \"timestamp\": 12345678 }").ToJsonDictionary();
            var target = JsonDocument.Parse(Properties.Resources.json05).ToJsonDictionary();

            var merged = source.MergeTo(target);
            Assert.AreEqual(2, merged.Count);

            var menu = merged.GetValueDictionary("menu");
            Assert.IsNotNull(menu);
            Assert.AreEqual("Super Viewer", menu["header"]);
        }



        [TestMethod]
        public void ToDictionary01()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json01);
            var d = doc.ToJsonDictionary();

            Assert.AreEqual(1, d.Count);

            var glossary = d.GetValueDictionary("glossary");
            Assert.IsNotNull(glossary);

            var glossDiv = glossary.GetValueDictionary("GlossDiv");
            Assert.IsNotNull(glossDiv);
        }

        [TestMethod]
        public void ToDictionary02()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json02);
            var d = doc.ToJsonDictionary();

            var menu = d.GetValueDictionary("menu");
            Assert.IsNotNull(menu);
        }

        [TestMethod]
        public void ToDictionary03()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json03);
            var d = doc.ToJsonDictionary();

            var window = d.GetValueDictionary("widget")?.GetValueDictionary("window");
            var image = d.GetValueDictionary("widget")?.GetValueDictionary("image");
            var text = d.GetValueDictionary("widget")?.GetValueDictionary("text");
        }

        [TestMethod]
        public void ToDictionary04()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json04);
            var d = doc.ToJsonDictionary();

            var arr = d.GetValueDictionary("web-app")?.GetValueList("servlet");
            Assert.IsNotNull(arr);
            Assert.AreEqual(5, arr.Count);
        }

        [TestMethod]
        public void ToDictionary05()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json05);
            var d = doc.ToJsonDictionary();
        }

        [TestMethod]
        public void ToDictionary06()
        {
            var doc = JsonDocument.Parse("{ \"int\": 5, \"null\": null }");
            var d = doc.ToJsonDictionary();

            Assert.AreEqual(2, d.Count);
            Assert.AreEqual(5, d["int"]);
            Assert.IsNull(d["null"]);
        }

        [TestMethod]
        public void ToDictionary07()
        {
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                EmployeeNumber = "1234"
            };

            var d = JsonUtil.CreateDictionary(employee, options: new JsonSerializerOptions());
            Assert.AreEqual(employee.FirstName, d[nameof(employee.FirstName)]);
            Assert.AreEqual(employee.LastName, d[nameof(employee.LastName)]);
            Assert.AreEqual(employee.EmployeeNumber, d[nameof(employee.EmployeeNumber)]);
        }

        [TestMethod]
        public void ToDictionary08()
        {
            var employee = new Employee
            {
                FirstName = "First",
                LastName = "Last"
            };

            var obj = JsonObject.Create(JsonSerializer.SerializeToElement(employee)) ?? throw new NullReferenceException();
            var d = obj.ToJsonDictionary();

            Assert.AreEqual(employee.FirstName, d[nameof(Employee.FirstName)]);
            Assert.AreEqual(employee.LastName, d[nameof(Employee.LastName)]);
        }

        [TestMethod]
        public void ToDictionary09()
        {
            var d = Newton.JToken.Parse("{ \"foo\": \"bar\" }").ToJsonDictionary();
            Assert.IsNotNull(d);
            Assert.IsTrue(d.ContainsKey("foo"));
            Assert.AreEqual("bar", d["foo"]);
        }

        [TestMethod]
        public void ToDictionary10()
        {
            var d = Newton.JObject.Parse(Properties.Resources.json01).ToJsonDictionary();
            Assert.IsNotNull(d);
            Assert.AreNotEqual(0, d.Count);
            Assert.IsNotNull(d["glossary"]);
        }

        [TestMethod]
        public void ToDictionary11()
        {
            var d = Newton.JToken.Parse("{\"arr\": [\"item1\", \"item2\"]}").ToJsonDictionary();
            Assert.IsNotNull(d?["arr"]);
            var list = d["arr"] as JsonList;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            CollectionAssert.Contains(list, "item1");
            CollectionAssert.Contains(list, "item2");
        }

        [TestMethod]
        public void ToDictionary12()
        {
            var d = Newton.JObject.Parse(Properties.Resources.json06).ToJsonDictionary();
            Assert.IsNotNull(d);

            var obj = d["object"] as JsonDictionary;
            Assert.IsNotNull(obj);
            Assert.AreEqual(true, obj["bool"]);
            Assert.AreEqual(3L, obj["int"]);
            Assert.AreEqual(3.141592, obj["decimal"]);
            Assert.AreEqual("Hello World!", obj["string"]);
            Assert.AreEqual("2022-03-09", obj["date"]);
            Assert.AreEqual(DateTime.Parse("2022-03-09T23:59:00Z").ToUniversalTime(), obj["dateTime"]);

            var arr = d["array"] as JsonList;
            Assert.IsNotNull(arr);
            CollectionAssert.Contains(arr, "string");
            CollectionAssert.Contains(arr, false);
            CollectionAssert.Contains(arr, 14L);
            CollectionAssert.Contains(arr, 1.5);
        }

        [TestMethod]
        public void ToDictionary13()
        {
            var response = new EndpointResponse
            {
                Endpoint = new Endpoint { Uri = "http://localhost" }
            };

            var json = JsonSerializer.Serialize(response, options: SerializationOptions);
            var dictionary = JsonUtil.CreateDictionary(json, options: SerializationOptions);
            Assert.IsNotNull(dictionary);
            JsonDictionary? endpoint = dictionary["endpoint"] as JsonDictionary;
            Assert.IsNotNull(endpoint);
            Assert.AreEqual(response.Endpoint.Uri, endpoint["uri"]);
        }

        [TestMethod]
        [ExpectedException(typeof(JsonException), AllowDerivedTypes = true)]
        public void ToDictionary14()
        {
            var dictionary = JsonUtil.CreateDictionary("foo");
        }

        [TestMethod]
        public void ToDictionary15()
        {
            var response = new EndpointResponse
            {
                Meta = new JsonDictionary
                {
                    { "isCool", true },
                    { "maxCount", 1024 },
                    { "etag", "1008087245692" }
                }
            };

            var dictionary = JsonUtil.CreateDictionary(response);
            var result = dictionary.Deserialize<EndpointResponse>();

            foreach(var key in response.Meta.Keys)
            {
                Assert.AreEqual(response.Meta[key], result.Meta[key], $"Comparing values with key '{key}'.");
            }
        }

        [TestMethod]
        [Description("Assert that by default, objects are serialized using camelCase.")]
        public void ToDictionary16()
        {
            var emp = new Employee { EmployeeNumber = "1234" };
            var dictionary = JsonUtil.CreateDictionary(emp);
            var num = dictionary["employeeNumber"];
            Assert.AreEqual(emp.EmployeeNumber, num);
        }

        [TestMethod]
        public void ToDictionary17()
        {
            JsonElement? elem = null;
            var dict = elem.ToJsonDictionary();
            Assert.IsNull(dict);
        }


        [TestMethod]
        public void ToList01()
        {
            var source = new List<Employee>
            {
                new Employee { FirstName = "John", LastName = "Doe" },
                new Employee { FirstName = "Jane", LastName= "Doe" }
            };

            var list = JsonUtil.CreateList(source, options: new JsonSerializerOptions());
            Assert.AreEqual(2, list.Count);

            for(var i = 0; i < source.Count; i++)
            {
                var item = (JsonDictionary)list[i];
                Assert.AreEqual(source[i].FirstName, item[nameof(Employee.FirstName)]);
                Assert.AreEqual(source[i].LastName, item[nameof(Employee.LastName)]);
            }
        }

        [TestMethod]
        public void ToList02()
        {
            var source = new List<Employee>
            {
                new Employee { FirstName = "Mr.", LastName = "Miyagi" },
                new Employee { FirstName = "Mrs.", LastName = "Miyagi" }
            };

            var elem = JsonSerializer.SerializeToElement(source);
            var arr = JsonArray.Create(elem) ?? throw new NullReferenceException();

            var list = arr.ToJsonList();
            Assert.AreEqual(source.Count, list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                var item = (JsonDictionary)list[i] ?? throw new NullReferenceException();
                Assert.AreEqual(source[i].FirstName, item[nameof(Employee.FirstName)]);
                Assert.AreEqual(source[i].LastName, item[nameof(Employee.LastName)]);
            }
        }

        [TestMethod]
        [Description("Assert that by default, lists are serialized using camelCase.")]
        public void ToList03()
        {
            var source = new List<Employee> { new Employee { EmployeeNumber = "007" } };
            var list = JsonUtil.CreateList(source);
            var emp = list.First() as JsonDictionary;
            
            Assert.IsNotNull(emp);
            var num = emp["employeeNumber"];
            Assert.AreEqual(source.First().EmployeeNumber, num);
        }
    }
}