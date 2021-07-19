using System;
using System.Collections;
using System.Collections.Generic;
using EventStreamProcessingTests.TestHelpers;
using EventStreamProcessing.Models;

namespace EventStreamProcessingTests.TestData
{
    public class DebatcherDeadLetterTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                            "foo bar",
                            "not a sensor"
                        },
                        EnqueuedTimeUtc = "2020-12-15T01:30:45Z",
                        Offset = "1",
                        PartitionId = "0"
                    },
                    "alpha1",
                    1
            };

            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                            "<sensor id='945032' type='alpha1' timestamp='1607242980'><value>sensor1</value></sensor>",
                            "<sensor id='351378' type='alpha2' timestamp='1607123953'><value>sensor2</value></sensor>",
                            "<sensor id='231173' type='beta1' timestamp='1607245800'><value>sensor3</value></sensor>",
                            "<sensor id='141597' type='alpha2' timestamp='1607165057'><value>sensor2</value></sensor>",
                            "not a sensor"
                        },
                        EnqueuedTimeUtc = "2020-12-15T01:30:45Z",
                        Offset = "2",
                        PartitionId = "0"
                    },
                    "alpha1",
                    0
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
