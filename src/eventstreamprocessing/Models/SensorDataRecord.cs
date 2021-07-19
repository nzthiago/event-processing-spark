using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventStreamProcessing.Models
{
    public class SensorDataRecord
    {
        public Sensor Sensor { get; set; }

        public DateTime EnqueuedTime { get; set; }

        public DateTime ProcessedTime { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}
