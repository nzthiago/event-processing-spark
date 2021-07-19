using System;
using System.Collections;
using System.Collections.Generic;
using EventStreamProcessingTests.TestHelpers;
using EventStreamProcessing.Models;

namespace EventStreamProcessingTests.TestData
{
    public class TransformerPayloadTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                            @"<sensor id=""945032"" type=""alpha1"" timestamp=""1607242980""><value>sensor1</value></sensor>",
                            @"<sensor id=""351378"" type=""alpha2"" timestamp=""1607123953""><value>sensor2</value></sensor>",
                            @"<sensor id=""231173"" type=""beta1"" timestamp=""1607245800""><value>sensor3</value></sensor>",
                            @"<sensor id=""141597"" type=""alpha2"" timestamp=""1607165057""><value>sensor2</value></sensor>"
                        },
                        EnqueuedTimeUtc = "2020-12-15T01:30:45Z",
                        Offset = "1",
                        PartitionId = "0"
                    },
                    new Sensor[] {
                        new Sensor() {Id = "945032", Type="alpha1", Timestamp = "1607242980", Value = "sensor1" },
                        new Sensor() {Id = "351378", Type="alpha2", Timestamp = "1607123953", Value = "sensor2" },
                        new Sensor() {Id = "231173", Type="beta1", Timestamp = "1607245800", Value = "sensor3" },
                        new Sensor() {Id = "141597", Type="alpha2", Timestamp = "1607165057", Value = "sensor2" }
                    }
            };

            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                            @"<sensor id=""945032"" type=""alpha1"" timestamp=""1607242980""><value>sensor1</value></sensor>",
                            @"<sensor id=""351378"" type=""alpha2"" timestamp=""1607123953""><value>sensor2</value></sensor>"
                        },
                        EnqueuedTimeUtc = "2020-12-15T01:30:45Z",
                        Offset = "2",
                        PartitionId = "0"
                    },
                    new Sensor[] {
                        new Sensor() {Id = "945032", Type="alpha1", Timestamp = "1607242980", Value = "sensor1" },
                        new Sensor() {Id = "351378", Type="alpha2", Timestamp = "1607123953", Value = "sensor2" }
                    }            
            };

            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                        },
                        EnqueuedTimeUtc = "2020-12-15T01:30:45Z",
                        Offset = "1",
                        PartitionId = "0"
                    },
                    new Sensor[] {}
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}