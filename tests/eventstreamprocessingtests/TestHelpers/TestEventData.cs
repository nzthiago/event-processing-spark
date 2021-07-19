using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStreamProcessing.Models;

namespace EventStreamProcessingTests.TestHelpers
{
    public class TestEventData {
        public List<string> Sensors;
        
        public string EnqueuedTimeUtc;
        
        public string Offset;

        public string PartitionId;

        public EventData[] CreateDispatcherEventData()
        {
            List<EventData> eventData = new List<EventData>();

            StringBuilder sb = new StringBuilder(@"<devices>");
            foreach(var sensor in this.Sensors) {
                sb.Append(sensor);
            }
            sb.Append("</devices>");    

            var data = new EventData(Encoding.UTF8.GetBytes(sb.ToString()));
            data.SystemProperties = new EventData.SystemPropertiesCollection(0, DateTime.Parse(this.EnqueuedTimeUtc), this.Offset, ""); ;
            eventData.Add(data);

            return eventData.ToArray();
        }

        public EventData[] CreateTransformerEventData()
        {
            List<EventData> eventData = new List<EventData>();

            foreach(var sensor in this.Sensors) {
                var data = new EventData(Encoding.UTF8.GetBytes(sensor));
                data.SystemProperties = new EventData.SystemPropertiesCollection(0, DateTime.Parse(this.EnqueuedTimeUtc), this.Offset, "");
                data.Properties.Add("InputEH_EnqueuedTimeUtc", DateTime.UtcNow);
                eventData.Add(data);
            }

            return eventData.ToArray();
        }
    }
}