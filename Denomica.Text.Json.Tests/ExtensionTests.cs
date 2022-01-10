using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using JSON = Denomica.Text.Json;

namespace Denomica.Text.Json.Tests
{
    using JsonDictionary = Dictionary<string, object?>;
    using JsonList = List<object?>;

    [TestClass]
    public class ExtensionTests
    {

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

            var d = JsonUtil.CreateDictionary(employee);
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
        public void ToList01()
        {
            var source = new List<Employee>
            {
                new Employee { FirstName = "John", LastName = "Doe" },
                new Employee { FirstName = "Jane", LastName= "Doe" }
            };

            var list = JsonUtil.CreateList(source);
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
    }
}