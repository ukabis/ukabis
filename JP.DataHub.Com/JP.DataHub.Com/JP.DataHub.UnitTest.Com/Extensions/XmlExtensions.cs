using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class XmlExtensions
    {
        public static string WILDCARD = "{{*}}";

        public static XElement ToXml(this string value)
        {
            var xml = new XmlDocument();
            xml.LoadXml(value);
            return XElement.Parse(xml.DocumentElement.OuterXml);
        }

        public static T ToXml<T>(this string value)
        {
            var xml = new XmlDocument();
            xml.LoadXml(value);

            var json = JToken.Parse(JsonConvert.SerializeXmlNode(xml.DocumentElement, Newtonsoft.Json.Formatting.None, true));
            return (T)JsonConvert.DeserializeObject(json.ToString(), typeof(T), new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
        }

        public static XElement StringToXml(this string value)
        {
            return XElement.Parse(value);
        }
        public static XElement JsonToXml(this string json)
        {

            using (var reader = JsonReaderWriterFactory.CreateJsonReader(new UTF8Encoding(false).GetBytes(json.Replace("'", "\"")), XmlDictionaryReaderQuotas.Max))
            {
                StringWriter sw = new StringWriterUTF8();
                var tempXml = XElement.Load(reader);
                tempXml.Attributes("type").Remove();
                foreach (var element in tempXml.Descendants().Where(e => e.Attribute("type") != null))
                {
                    element.Attributes("type").Remove();
                }
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new System.Text.UTF8Encoding(false);
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
                {
                    tempXml.WriteTo(xmlWriter);
                }
                return XElement.Parse(sw.ToString());
            }

        }

        public static void Is(this XElement actual, XElement expected, string message = "")
        {
            ReplaceWildCardElement(actual, GetIgnoreName(expected));
            message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
            if (object.ReferenceEquals(actual, expected)) return;

            if (actual == null) throw new AssertFailedException("actual is null" + message);
            if (expected == null) throw new AssertFailedException("actual is not null" + message);
            if (actual.GetType() != expected.GetType())
            {
                var msg = string.Format("expected type is {0} but actual type is {1}{2}",
                    expected.GetType().Name, actual.GetType().Name, message);
                throw new AssertFailedException(msg);
            }
            if (actual.Elements().Count() != expected.Elements().Count())
            {
                var msg = string.Format("expected type is {0} but actual type is {1}{2}", expected.GetType().Name, actual.GetType().Name, message);
                throw new AssertFailedException(msg);
            }

            if (!XNode.DeepEquals(actual, expected))
            {
                var msg = string.Format("expected type  but actual type ", message);
                throw new AssertFailedException(msg);

            }

        }

        private static List<string> GetIgnoreName(XElement element)
        {
            List<string> names = new List<string>();
            foreach (var el in element.Elements())
            {
                if (el.HasElements)
                {
                    names.AddRange(GetIgnoreName(el));
                }
                if (el.Value == WILDCARD)
                {
                    names.Add(el.Name.ToString());
                }
            }
            return names.Distinct().ToList();
        }
        private static XElement ReplaceWildCardElement(XElement element, List<string> ignoreNames)
        {
            foreach (var el in element.Elements())
            {
                if (el.HasElements)
                {
                    ReplaceWildCardElement(el, ignoreNames);
                }
                if (ignoreNames.Contains(el.Name.ToString()))
                {
                    el.Value = WILDCARD;
                }
            }
            return element;
        }


        private class StringWriterUTF8 : StringWriter
        {
            public override System.Text.Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }
        }
    }
}
