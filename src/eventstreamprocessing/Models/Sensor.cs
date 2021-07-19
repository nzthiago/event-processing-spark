using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace EventStreamProcessing.Models
{
	[XmlRoot(ElementName = "sensor")]
	public class Sensor
	{
		[XmlElement(ElementName = "value")]
		public string Value { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "timestamp")]
		public string Timestamp { get; set; }
	}
}
