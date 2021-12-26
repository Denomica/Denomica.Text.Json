using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;
using JSON = Denomica.Text.Json;

namespace Denomica.Text.Json.Tests
{
    [TestClass]
    public class ExtensionTests
    {
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