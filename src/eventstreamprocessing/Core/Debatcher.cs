using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using EventStreamProcessing.Helpers.Extensions;
using EventStreamProcessing.Helpers;
using System.Collections.Generic;

namespace EventStreamProcessing.Core {
    public class Debatcher {
        private ILogger _log;

        public Debatcher(ILogger log) {
            _log = log;    
        }

        public EventData[] Debatch(EventData[] inputMessages, string partitionId) {
            _log.LogBatchSize("DebatchingFunction", inputMessages.Length);

            List<EventData> outputMessages = new List<EventData>();

            foreach (EventData message in inputMessages)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(message.Body.Array,
                                                              message.Body.Offset,
                                                              message.Body.Count);

                    var messageXml = XDocument.Parse(messageBody);
                    var sensorType = Environment.GetEnvironmentVariable("SENSOR_TYPE");
                    var sensors = messageXml.XPathSelectElements($"//devices/sensor[starts-with(@type, '{sensorType}')]");

                    if (!sensors.Any())
                    {
                        // Log that we're skipping this message
                        _log.LogInformation($"No sensors matching {sensorType} in this message, skipping");
                    }
                    else
                    {
                        foreach (XElement sensor in sensors)
                        {
                            var outputMessage = new EventData(Encoding.UTF8.GetBytes(
                                                    Conversion.ConvertXmlToString(sensor)));

                            outputMessage.Properties.Add("InputEH_EnqueuedTimeUtc",
                                message.SystemProperties["x-opt-enqueued-time"]);
                            outputMessage.SystemProperties = message.SystemProperties;
                            // Add output message and increment count
                            outputMessages.Add(outputMessage);
                            
                            // Log success
                            _log.LogEventProcessed(partitionId, message);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.LogProcessingError(e, "DebatchingFunction", partitionId, message);
                }
            }

            _log.LogProcessingComplete("DebatchingFunction", inputMessages.Length, outputMessages.Count, partitionId);
            return outputMessages.ToArray();
        }
    }
}