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
            return hostBuilder
            .AddMemoryGrainStorage("PubSubStore")
            .AddKafka("KafkaStreamProvider")
            .WithOptions(options =>
            {
                options.BrokerList = new[] { Constants.KafkaHost };
                options.ConsumerGroupId = "orleans-kafka";
                options.Topics = new List<TopicConfig> { new TopicConfig { Name = Constants.KafkaTopic } };
                options.MessageTrackingEnabled = true;
            })
            .AddJson()
            .AddLoggingTracker()
            .Build();
        }
    }
}
