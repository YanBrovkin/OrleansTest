using Domain.Constants;
using Orleans;
using Orleans.Hosting;

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
                    options.ConsumerGroupId = "E2EGroup";

                    options
                        .AddTopic(Constants.KafkaTopic);
                })
                .AddJson()
                .Build();
        }
    }
}
