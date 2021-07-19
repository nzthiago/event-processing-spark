using System.Collections;
using System.Collections.Generic;
using EventStreamProcessingTests.TestHelpers;

namespace EventStreamProcessingTests.TestData
{
    public class TransformerNullSensorValuesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                    new TestEventData() {
                        Sensors = new List<string>{
                            @"<sensor id=""945032"" type=""alpha1"" timestamp=""1607242980""><value></value></sensor>",
                            @"<sensor id=""351378"" type=""alpha2"" timestamp=""1607123953""><value>sensor2</value></sensor>",
                            @"<sensor id=""231173"" type=""beta1"" timestamp=""1607245800""></sensor>",
                            @"<sensor id=""141597"" type=""alpha2"" timestamp=""1607165057""><value>sensor2</value></sensor>"
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
