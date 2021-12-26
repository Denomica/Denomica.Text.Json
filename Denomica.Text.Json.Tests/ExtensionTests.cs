using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JSON = Denomica.Text.Json;

namespace Denomica.Text.Json.Tests
{
    [TestClass]
    public class ExtensionTests
    {

        [TestMethod]
        public void MergeList01()
        {
            var source = new ValueList { 1, 2, 3, 4 };
            var target = new ValueList { 5, 6, 7, 8 };

            var merged = source.MergeTo(target);
            Assert.AreEqual(8, merged.Count);
            CollectionAssert.AllItemsAreUnique(merged);
        }

        [TestMethod]
        public void MergeList02()
        {
            var source = new ValueList
            {
                1,
                3.141592653589793238,
                new ValueDictionary
                {
                    { "id", "value" }
                },
                true
            };
            var target = new ValueList
            {
                true,
                new ValueDictionary
                {
                    { "id", "value" }
                }
            };

            var merged = source.MergeTo(target);
            Assert.AreEqual(5, merged.Count);

            var dictionaries = from x in merged where x is ValueDictionary select x;
            Assert.AreEqual(2, dictionaries.Count());

            CollectionAssert.Contains(merged, 1);
            CollectionAssert.Contains(merged, true);
            CollectionAssert.Contains(merged, 3.141592653589793238);
        }



        [TestMethod]
        public void MergeDictionary01()
        {
            var source = JsonDocument.Parse(Properties.Resources.json02a).ToDictionary();
            var target = JsonDocument.Parse(Properties.Resources.json02).ToDictionary();

            var merged = source.MergeTo(target);
            var menu = merged.GetValueDictionary("menu");

            Assert.IsNotNull(menu);
            Assert.AreEqual("file-a", menu["id"]);
        }

        [TestMethod]
        public void MergeDictionary02()
        {
            var source = JsonDocument.Parse("{ \"menu\": { \"header\": \"Super Viewer\" }, \"timestamp\": 12345678 }").ToDictionary();
            var target = JsonDocument.Parse(Properties.Resources.json05).ToDictionary();

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
            var d = doc.ToDictionary();

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
            var d = doc.ToDictionary();

            var menu = d.GetValueDictionary("menu");
            Assert.IsNotNull(menu);
        }

        [TestMethod]
        public void ToDictionary03()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json03);
            var d = doc.ToDictionary();

            var window = d.GetValueDictionary("widget")?.GetValueDictionary("window");
            var image = d.GetValueDictionary("widget")?.GetValueDictionary("image");
            var text = d.GetValueDictionary("widget")?.GetValueDictionary("text");
        }

        [TestMethod]
        public void ToDictionary04()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json04);
            var d = doc.ToDictionary();

            var arr = d.GetValueDictionary("web-app")?.GetValueList("servlet");
            Assert.IsNotNull(arr);
            Assert.AreEqual(5, arr.Count);
        }

        [TestMethod]
        public void ToDictionary05()
        {
            var doc = JsonDocument.Parse(Properties.Resources.json05);
            var d = doc.ToDictionary();
        }

        [TestMethod]
        public void ToDictionary06()
        {
            var doc = JsonDocument.Parse("{ \"int\": 5, \"null\": null }");
            var d = doc.ToDictionary();

            Assert.AreEqual(2, d.Count);
            Assert.AreEqual(5, d["int"]);
            Assert.IsNull(d["null"]);
        }

    }
}