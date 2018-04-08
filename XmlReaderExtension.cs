using System;
using System.Xml;

namespace TestTask_Nival
{
    public static class XmlReaderExtension
    {
        public static string ReadAttribute(this XmlReader reader, string attributeName)
        {
            if (reader.MoveToAttribute(attributeName))
                return reader.ReadContentAsString();
            else
                throw GetXmlException(reader, string.Format("Attribute '{0}' was not found", attributeName));
        }

        public static string ReadEmptyNameValueNode(this XmlReader reader, string name)
        {
            if(reader.ReadAttribute("name").Equals(name))
                return reader.ReadAttribute("value");
            else
                throw GetXmlException(reader, string.Format("Element '{0}' was not found", name));
        }

        private static XmlException GetXmlException(XmlReader reader, string message)
        {
            var lineInfo = reader as IXmlLineInfo;
            if (lineInfo == null)
                throw new XmlException(message);
            else
                throw new XmlException(message, null, lineInfo.LineNumber, lineInfo.LinePosition);
        }
    }
}
