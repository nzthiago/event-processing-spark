using System.Collections;
using System.Collections.Generic;
using EventStreamProcessingTests.TestHelpers;

namespace EventStreamProcessingTests.TestData
{
    public class TransformerInvalidSensorTestData : IEnumerable<object[]>
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
                    2
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
