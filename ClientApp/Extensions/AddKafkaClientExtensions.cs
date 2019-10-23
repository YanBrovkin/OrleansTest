using System.Collections.Generic;
using Domain.Constants;
using Orleans;
using Orleans.Hosting;
using Orleans.Streams.Kafka.Config;

namespace ClientApp.Extensions
{
    public static class AddKafkaClientExtensions
    {
        public static IClientBuilder AddKafkaConfig(this IClientBuilder clientBuilder)
        {
            return clientBuilder
                .AddKafka("KafkaStreamProvider")
                .WithOptions(options =>
                {
                    options.BrokerList = new[] { Constants.KafkaHost };
                    options.ConsumerGroupId = "orleans-kafka";
                    options.Topics = new List<TopicConfig> { new TopicConfig { Name = Constants.KafkaTopic } };

                    //options.AddTopic(Constants.KafkaTopic);
                })
                .AddJson()
                .Build();
        }
    }
}
