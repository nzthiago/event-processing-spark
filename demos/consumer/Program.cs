using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

namespace Consumer
{
    class Program
    {
        private static EventHub _eventHub;
        private static BlobStorage _blobStorage;

        static async Task Main(string[] args)
        {
             IConfiguration Configuration = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: false)
                                                .AddJsonFile("appsettings.Local.json", optional: true)
                                                .AddEnvironmentVariables()
                                                .AddCommandLine(args)
                                                .Build();

            _eventHub = Configuration.GetSection("EventHub").Get<EventHub>();
            _blobStorage = Configuration.GetSection("BlobStorage").Get<BlobStorage>();

            await ReadEvents();
            while(true) {
                Console.WriteLine(@"Awaiting events (""Ctrl+C"" to Quit)... ");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private static async Task ReadEvents() {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            BlobContainerClient storageClient = new BlobContainerClient(_blobStorage.ConnectionString, _blobStorage.Container);
            storageClient.CreateIfNotExists();
            
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, _eventHub.ConnectionString + $";EntityPath={_eventHub.EventHubName}");

            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            await processor.StartProcessingAsync();
        }

        private static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\nReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        private static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }  
    }
}
