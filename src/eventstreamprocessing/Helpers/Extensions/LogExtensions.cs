using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;

namespace EventStreamProcessing.Helpers.Extensions
{
    public static class LogExtensions
    {
        public static void LogBatchSize(this ILogger logger, string functionName, int batchSize)
        {
            logger.LogInformation($"{functionName}: batchSize={batchSize}");
        }

        public static void LogEventProcessed(this ILogger logger, string partitionId, EventData message){
            var processedTime = DateTime.UtcNow;
            var enqueuedTimeUtc = message.SystemProperties.EnqueuedTimeUtc;
            var debatchingLatencyInMs = TimeCalculations.GetLatency(enqueuedTimeUtc, processedTime);

            logger.LogInformation("DebatchingFunction: Processed message with partitionId={partitionId}, " +
                "offset={offset}, processedTime={processedTime} enqueuedTimeUtc={enqueuedTimeUtc}, " +
                "debatchingLatencyInMs={debatchingLatencyInMs}",
                partitionId,
                message.SystemProperties.Offset,
                processedTime,
                enqueuedTimeUtc,
                debatchingLatencyInMs
            );
        }

        public static void LogSensorProcessed(this ILogger logger, string sensorDataJson, string partitionId, DateTime processedTime, EventData message)
        {
            var enqueuedTimeUtc = message.SystemProperties.EnqueuedTimeUtc;
            DateTime inputEH_enqueuedTime = (DateTime)message.Properties["InputEH_EnqueuedTimeUtc"];

            var transformingLatencyInMs = TimeCalculations.GetLatency(enqueuedTimeUtc, processedTime);
            var processingLatencyInMs = TimeCalculations.GetLatency(inputEH_enqueuedTime, processedTime);

            logger.LogInformation("TransformingFunction: Processed sensorDataJson={sensorDataJson}, " +
                "partitionId={partitionId}, offset={offset} at {enqueuedTimeUtc}, " +
                "inputEH_enqueuedTime={inputEH_enqueuedTime}, processedTime={processedTime}, " +
                "transformingLatencyInMs={transformingLatencyInMs}, processingLatencyInMs={processingLatencyInMs}",
                sensorDataJson,
                partitionId,
                message.SystemProperties.Offset,
                enqueuedTimeUtc,
                inputEH_enqueuedTime,
                processedTime,
                transformingLatencyInMs,
                processingLatencyInMs);
        }

        public static void LogProcessingError(this ILogger logger, Exception exc, string functionName, string partitionId, EventData message)
        {
            message.Properties.Add("error", exc?.Message);
            message.Properties.Add("stacktrace", exc?.StackTrace);

            logger.LogError(exc, $"{functionName}: Failed processing message with partitionId={partitionId}, offset={message.SystemProperties.Offset}");
        }

        public static void LogProcessingComplete(this ILogger logger, string functionName, int originalMessageCount, int successfulMessageCount, string partitionId)
        {
            logger.LogInformation($"{functionName}: successfulMessageCount={successfulMessageCount}, partitionId={partitionId}, batchSize={originalMessageCount}");
        }
    }
}