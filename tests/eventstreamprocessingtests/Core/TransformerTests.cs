using EventStreamProcessing.Helpers;
using EventStreamProcessing.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;
using System.Linq;
using Xunit;
using EventStreamProcessingTests.TestHelpers;
using EventStreamProcessingTests.TestData;
using EventStreamProcessing.Core;
using System.Collections.Generic;

namespace EventStreamProcessingTests.Core
{
    public class TransformerTests
    {
        [Theory]
        [ClassData(typeof(TransformerPayloadTestData))]
        public async void Transformer_TestPayloads_ReturnsCreatedSensors(TestEventData data, Sensor[] returnedSensors)
        {
            // Arrange
            var outputMessages = new TestAsyncCollector<SensorDataRecord>();

            var eventData = data.CreateTransformerEventData();

            // Act
            var xformer = new Transformer(new Mock<ILogger>().Object);
            await xformer.Transform(eventData, data.PartitionId, outputMessages);

            // Assert
            Assert.Equal(returnedSensors.Length, outputMessages.Values.Count);
            for(var i = 0; i < outputMessages.Values.Count; i++) {
                var expectedSensor = returnedSensors[i];
                var outputSensor = outputMessages.Values.GetRange(i, 1).Single().Sensor;

                Assert.Equal(expectedSensor.Id, outputSensor.Id);
                Assert.Equal(expectedSensor.Type, outputSensor.Type);
                Assert.Equal(expectedSensor.Timestamp, outputSensor.Timestamp);
                Assert.Equal(expectedSensor.Value, outputSensor.Value);
            }
        }

        [Theory]
        [ClassData(typeof(TransformerInvalidSensorTestData))]
        public async void Transformer_NoValidSensors_ThrowsError(TestEventData data, int numberOfErrors)
        {
            // Arrange
            var outputMessages = new TestAsyncCollector<SensorDataRecord>();
            var loggerMock = new Mock<ILogger>();

            var eventData = data.CreateTransformerEventData();

            // Act
            var xformer = new Transformer(loggerMock.Object);
            await xformer.Transform(eventData, data.PartitionId, outputMessages);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"TransformingFunction: Failed processing message with partitionId={data.PartitionId}, offset={data.Offset}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                Times.Exactly(numberOfErrors));
        }

        [Theory]
        [ClassData(typeof(TransformerNullSensorValuesTestData))]
        public async void Transformer_NullSensorValues_ThrowsError(TestEventData data, int numberOfErrors)
        {
            // Arrange
            var outputMessages = new TestAsyncCollector<SensorDataRecord>();
            var loggerMock = new Mock<ILogger>();

            var eventData = data.CreateTransformerEventData();

            // Act
            var xformer = new Transformer(loggerMock.Object);
            await xformer.Transform(eventData, data.PartitionId, outputMessages);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"TransformingFunction: Failed processing message with partitionId={data.PartitionId}, offset={data.Offset}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<ArgumentNullException>(),
                    (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                Times.Exactly(numberOfErrors));
        }
    }
}
