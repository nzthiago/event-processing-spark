using System.Threading.Tasks;
using EventStreamProcessing.Core;
using EventStreamProcessing.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventStreamProcessing.Functions
{
    public static class ProcessorFunction
    {
        [FunctionName("ProcessorFunction")]
        public static async Task Run(
            [EventHubTrigger(
                "%Input_EH_Name%",
                Connection = "InputEventHubConnectionString",
                ConsumerGroup = "%Input_EH_ConsumerGroup%")] EventData[] inputMessages,
            [EventHub(
                "%Output_EH_Name%",
                Connection = "OutputEventHubConnectionString")] IAsyncCollector<SensorDataRecord> outputMessages,
            PartitionContext partitionContext,
            ILogger log)
        {
            var debatcher = new Debatcher(log);
            var debatchedMessages = debatcher.Debatch(inputMessages, partitionContext.PartitionId);

            var xformer = new Transformer(log);
            await xformer.Transform(debatchedMessages, partitionContext.PartitionId, outputMessages);
        }
    }
}
