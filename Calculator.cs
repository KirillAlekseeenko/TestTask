using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace TestTask_Nival
{
    [XmlRoot("folder")]
    public class Calculator : IXmlSerializable, IComparable
    {
        private List<Calculation> calculations;
        private StringWriter log;

        private Calculator() { }   // can only be deserialized

        public int CalculationsCount => calculations.Count;
        public string Log => log.ToString();
        public string Filename { get; set; }

        public void ReadXml(XmlReader reader)
        {
            calculations = new List<Calculation>();
            log = new StringWriter();

            while(!reader.EOF)
            {
                if(!(reader.MoveToAttribute("name") && reader.ReadContentAsString() == "calculation")) // to the next calculation node
                {
                    reader.Read();                        
                    continue;
                }
                try
                {
                    reader.ReadStartElement();

                    var uid = ReadUID(reader);
                    var operand = ReadOperand(reader);
                    var mod = ReadMod(reader);

                    calculations.Add(new Calculation(uid, operand, mod));

                    reader.ReadEndElement();
                }
                catch(Exception ex)
                {
                    var message = ex.Message;
                    if (ex is FormatException || ex is ArgumentException) // provide user with line and position info
                    {
                        if ((reader is IXmlLineInfo xmlInfo))
                            message += string.Format(" Line {0}, position {1}", xmlInfo.LineNumber, xmlInfo.LinePosition);
                    } 
                    else if(!(ex is XmlException))
                        throw;
                    
                    log.WriteLine(message);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema() => null;

        public int CompareTo(object obj)
        {
            if (obj is Calculator)
                return CalculationsCount.CompareTo(((Calculator)obj).CalculationsCount);
            else
                throw new ArgumentException("Object is not a Calculator");
        }

        public int Process()
        {
            int result = 0;
            foreach (var calculation in calculations)
            {
                result = calculation.Process(result);
            }
            return result;
        }

        private string ReadUID(XmlReader reader)
        {
            var uid = reader.ReadEmptyNameValueNode("uid");
            reader.ReadStartElement();
            return uid;
        }

        private Operand ReadOperand(XmlReader reader)
        {
            var operandString = reader.ReadEmptyNameValueNode("operand");
            var operand = (Operand)Enum.Parse(typeof(Operand), operandString.Substring(0, 1).ToUpper() + operandString.Substring(1));
            reader.ReadStartElement();
            return operand;
        }

        private int ReadMod(XmlReader reader)
        {
            var modString = reader.ReadEmptyNameValueNode("mod");
            var mod = Int32.Parse(modString);
            reader.ReadStartElement();
            return mod;
        }
    }
}
