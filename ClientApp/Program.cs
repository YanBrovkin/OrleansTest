using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Constants;
using Domain.Interfaces;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams.Kafka.Config;

namespace ClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientTask = StartClientWithRetries();

            var clusterClient = await clientTask;

            var userGrain = clusterClient.GetGrain<IUserGrain>(1);

            var result = await userGrain.SayHello("Hi");

            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(result);

            var streamProvider = clusterClient.GetStreamProvider("KafkaStreamProvider");
            var stream = streamProvider.GetStream<string>(Guid.Empty, "testTopic");
            Console.WriteLine("Ready to send messages ?");
            Console.ReadLine();
            var tasks = new List<Task>();
            // example of calling grains from the initialized client
            for (var i = 0; i <= 200; i++)
            {
                tasks.Add(stream.OnNextAsync($"Message: {i}"));
            }
            Task.WaitAll(tasks.ToArray());
            Console.ReadLine();
        }

        private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 7)
        {
            var attempt = 0;
            IClusterClient client;
            while (true)
            {
                try
                {
                    //var siloAddress = IPAddress.Loopback;
                    //var gatewayPort = 30000;

                    client = new ClientBuilder()
                        .UseLocalhostClustering()
                        //.Configure<ClusterOptions>(options =>
                        //{
                        //    options.ClusterId = "TestCluster";
                        //    options.ServiceId = "123";
                        //})
                        //.UseStaticClustering(options => options.Gateways.Add((new IPEndPoint(siloAddress, gatewayPort)).ToGatewayUri()))
                        //.ConfigureApplicationParts(parts => parts.AddApplicationPart(Assembly.Load("TestGrains")).WithReferences())
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UserGrain).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddConsole())
                        .AddKafka("KafkaStreamProvider")
                        .WithOptions(options =>
                        {
                            options.BrokerList = new List<string> { "localhost:9092" };
                            options.ConsumerGroupId = "orleans-kafka";
                            options.Topics = new List<TopicConfig> { new TopicConfig { Name = Constants.KafkaTopic } };
                        })
                        .Build()
                        .Build();

                    await client.Connect();

                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine(
                        $"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }

            return client;
        }
    }
}

