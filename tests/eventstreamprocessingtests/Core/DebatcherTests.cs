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
    public class DebatcherTests
    {
        [Theory]
        [ClassData(typeof(DebatcherPayloadTestData))]
        public void Debatcher_TestPayloads_ReturnsFilteredMessages(TestEventData data, string sensorType, int messageCount, string[] returnedSensors)
        {
            // Arrange
            var outputMessages = new List<EventData>();
            Environment.SetEnvironmentVariable("SENSOR_TYPE", sensorType);

            var eventData = data.CreateDispatcherEventData();

            // Act
            var debatcher = new Debatcher(new Mock<ILogger>().Object);
            outputMessages = debatcher.Debatch(eventData, data.PartitionId).ToList();

            // Assert
            Assert.Equal(messageCount, outputMessages.Count);
            Assert.Equal(returnedSensors, outputMessages.Select(v => Encoding.UTF8.GetString(v.Body)));
        }


        [Theory]
        [ClassData(typeof(DebatcherDeadLetterTestData))]
        public void Debatcher_NoSensors_ThrowsError(TestEventData data, string sensorType, int numberOfDeadLetters)
        {
            // Arrange
            var outputMessages = new List<EventData>();
            Environment.SetEnvironmentVariable("SENSOR_TYPE", sensorType);
            var loggerMock = new Mock<ILogger>();

            var eventData = data.CreateDispatcherEventData();

            // Act
            var debatcher = new Debatcher(loggerMock.Object);
           outputMessages = debatcher.Debatch(eventData, data.PartitionId).ToList();

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"DebatchingFunction: Failed processing message with partitionId={data.PartitionId}, offset={data.Offset}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                Times.Exactly(numberOfDeadLetters));
        }
       
        [Theory]
        [ClassData(typeof(DebatcherTimestampErrorTestData))]
        public void Debatcher_NoTimestamp_ThrowsError(TestEventData data, string sensorType, int numberOfErrors)
        {
            // Arrange
            var outputMessages = new List<EventData>();
            Environment.SetEnvironmentVariable("SENSOR_TYPE", sensorType);
            var loggerMock = new Mock<ILogger>();

            var eventData = data.CreateDispatcherEventData();

            // Act
            var debatcher = new Debatcher(loggerMock.Object);
           outputMessages = debatcher.Debatch(eventData, data.PartitionId).ToList();

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"DebatchingFunction: Failed processing message with partitionId={data.PartitionId}, offset={data.Offset}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<ArgumentException>(),
                    (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                Times.Exactly(numberOfErrors));
        }
    }
}
