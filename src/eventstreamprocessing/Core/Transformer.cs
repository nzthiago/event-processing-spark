using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using EventStreamProcessing.Helpers;
using EventStreamProcessing.Helpers.Extensions;
using EventStreamProcessing.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventStreamProcessing.Core {
    public class Transformer {
        private ILogger _log;

        public Transformer(ILogger log) {
            _log = log;    
        }

        public async Task Transform(EventData[] inputMessages, string partitionId, IAsyncCollector<SensorDataRecord> outputMessages) {
            // Initialize variables
            var outputMessageCount = 0;

            _log.LogBatchSize("TransformingFunction", inputMessages.Length);

            // Process each event in batch
            foreach (EventData message in inputMessages)
            {
                try
                {
                    // Get messageBody details
                    var messageBody = Encoding.UTF8.GetString(message.Body.Array,
                                                          message.Body.Offset,
                                                          message.Body.Count);

                    // Convert message body from XML string to object
                    var sensorDataObject = Conversion.ConvertXmlToSensorDataObject(messageBody);

                    // Get JSON string
                    var sensorDataJson = JsonConvert.SerializeObject(sensorDataObject);

                    // Checks if sensor details are available.
                    // This custom exception message can be used to determine processing
                    // of messages with missing sensor details
                    if (String.IsNullOrWhiteSpace(sensorDataObject?.Value))
                    {
                        throw new ArgumentNullException($"Sensor value missing for partitionId={partitionId}, offset={message.SystemProperties.Offset}");
                    }

                    // Create record to be entered into database
                    var processedTime = DateTime.UtcNow;
                    SensorDataRecord dataRecord = new SensorDataRecord()
                    {
                        Sensor = sensorDataObject,
                        EnqueuedTime = message.SystemProperties.EnqueuedTimeUtc,
                        ProcessedTime = processedTime,
                        RowKey = Guid.NewGuid().ToString(),
                        PartitionKey = sensorDataObject.Id
                    };

                    // Log success
                    _log.LogSensorProcessed(sensorDataJson, partitionId, processedTime, message);

                    await outputMessages.AddAsync(dataRecord);
                    outputMessageCount++;
                }
                catch (Exception e)
                {
                    _log.LogProcessingError(e, "TransformingFunction", partitionId, message);
                }
            }

            _log.LogProcessingComplete("TransformingFunction", inputMessages.Length, outputMessageCount, partitionId);
        }
    }
}