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
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Producer
{
    class Program
    {
        private static EventHub _eventHub;

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

            while(true) {
                Console.Write(@"Enter total number of event batches that should be sent (""q"" to Quit): ");
                var r = new Random();
                var batchCnt = r.Next(1,100);//Console.ReadLine();
                //if (batchCnt.ToLower() == "q") break;

                Console.Write(@"Enter number of test sensors that should be sent per batch: ");
                var sensorCnt = r.Next(1,10);// Console.ReadLine();

                var batches = await CreatePayloads(Convert.ToInt32(batchCnt), Convert.ToInt32(sensorCnt));
                Console.WriteLine("Test Payloads:");

                int i = 1;
                foreach(var batch in batches) {
                    Console.WriteLine($"** Batch {i++} **");
                    Console.WriteLine(batch);
                }

                SendPayload(batches).Wait();
            }
        }


        private static async Task<List<string>> CreatePayloads(int numOfBatches, int numOfSensors) {
            List<string> batches = new List<string>();
            List<string> randomWords = new List<string>();
            using (var httpClient = new HttpClient()) {
                var json = await httpClient.GetStringAsync("https://random-word-api.herokuapp.com/word?number=10&swear=0");
                randomWords = JsonConvert.DeserializeObject<List<string>>(json);
            }

            for (int i = 0; i < numOfBatches; i++) {
                StringBuilder sb = new StringBuilder(
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <devices>");
                sb.AppendLine();
                Enumerable.Range(1, numOfSensors).ToList().ForEach(s => sb.AppendLine(CreateSensor(randomWords)));
                sb.AppendLine("  </devices>");

                batches.Add(Regex.Replace(sb.ToString(), @"[ ]{5,}", "  "));
            }
            return batches;
        }

        private static string CreateSensor(List<string> randomWords) {
            var colors = new string[] { "blue", "green", "yellow", "red", "black", "white", "purple" };
            var random = new Random();
            var id = random.Next(100, 110);
            var color = colors[random.Next(0,6)];
            var timestamp = Convert.ToInt32((DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds + new Random().Next(0, 10000));
            var randomPhrase = string.Join(string.Empty,randomWords.OrderBy(x => Guid.NewGuid()).Take(3));
            return $"    <sensor id='{id}' type='{color}' timestamp='{timestamp}'><value>{randomPhrase}</value></sensor>".Replace("'", "\"");
        }

        private static async Task SendPayload(List<string> batches) {
            await using (var producerClient = new EventHubProducerClient(_eventHub.ConnectionString, _eventHub.EventHubName))
            {
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                foreach(var batch in batches)
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(batch)));

                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"Batches sent.");
                Console.WriteLine();
            }
        }
    }
}
