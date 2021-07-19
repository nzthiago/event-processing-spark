using EventStreamProcessing.Helpers;
using EventStreamProcessing.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;
using System.Linq;
using Xunit;
using EventStreamProcessing.Core;
using System.Xml.Linq;

namespace EventStreamProcessingTests.Helpers
{
    public class ConversionTests
    {
        [Fact]
        public void ConvertXmlToString_XmlElement_ReturnsString()
        {
            // Arrange
            XElement contacts = 
                new XElement("Contacts",
                    new XElement("Contact",
                        new XElement("Name", "Patrick Hines"),
                        new XElement("Phone", new XAttribute("Type", "Home"), "206-555-0144"),
                        new XElement("Address",
                            new XElement("Street1", "123 Main St"),
                            new XElement("City", "Mercer Island"),
                            new XElement("State", "WA"),
                            new XElement("Postal", "68042")
                        )
                    )
                );

            // Act
            var xmlString = Conversion.ConvertXmlToString(contacts);

            // Assert
            Assert.Equal("<Contacts><Contact><Name>Patrick Hines</Name><Phone Type=\"Home\">206-555-0144</Phone><Address><Street1>123 Main St</Street1><City>Mercer Island</City><State>WA</State><Postal>68042</Postal></Address></Contact></Contacts>", xmlString);
        }

        [Fact]
        public void ConvertXmlToSensorDataObject_TestMessage_ReturnsSensor()
        {
            // Arrange
            string xmlData = "<sensor id='945032' type='alpha1' timestamp='1607242980'><value>sensor1</value></sensor>";

            // Act
            var sensor = Conversion.ConvertXmlToSensorDataObject(xmlData);

            // Assert
            Assert.Equal("945032", sensor.Id);
            Assert.Equal("alpha1", sensor.Type);
            Assert.Equal("1607242980", sensor.Timestamp);
            Assert.Equal("sensor1", sensor.Value);
        }
    }
}