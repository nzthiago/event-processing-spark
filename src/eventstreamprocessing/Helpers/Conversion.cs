using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using EventStreamProcessing.Models;

namespace EventStreamProcessing.Helpers {
    public static class Conversion {
        public static string ConvertXmlToString(XElement xml) {
            var xmlWriter = new StringWriter();
            xml.WriteTo(new XmlTextWriter(xmlWriter));
            
            return xmlWriter.ToString();
        }

        public static Sensor ConvertXmlToSensorDataObject(string messageBody) {
            Sensor sensorDataObject;
            var xmlSerializer = new XmlSerializer(typeof(Sensor));

            using (StringReader textReader = new StringReader(messageBody))
            {
                sensorDataObject = (Sensor)xmlSerializer.Deserialize(textReader);
            }

            return sensorDataObject;
        }
    }
}