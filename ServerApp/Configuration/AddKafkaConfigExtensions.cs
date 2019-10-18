using System.Collections.Generic;
using System.Reflection;
using Domain.Constants;
using Orleans.Hosting;
using Orleans.Providers.Streams.Generator;
using Orleans.Streams.Kafka.Config;

namespace ServerApp.Configuration
{
    public static class AddKafkaConfigExtensions
    {
        public static ISiloBuilder AddKafkaConfig(this ISiloBuilder hostBuilder)
        {
            var result = hostBuilder
            //.ConfigureApplicationParts(parts => parts.AddApplicationPart(Assembly.Load("TestGrains")).WithReferences())
            .AddMemoryGrainStorage("PubSubStore")
            .AddKafka("KafkaProvider")
            .WithOptions(options =>
            {
                options.BrokerList = new[] { Constants.KafkaHost };
                options.ConsumerGroupId = "E2EGroup";
                //options.ConsumeMode = ConsumeMode.StreamEnd;
                options.Topics = new List<TopicConfig> { new TopicConfig { Name = Constants.KafkaTopic } };
                options.MessageTrackingEnabled = true;
            })
            .AddJson()
            .Build();
        }
    }
}
