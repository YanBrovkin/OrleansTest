using System;
using System.Threading.Tasks;
using Domain.Aggregates;
using Domain.Interfaces;
using Orleans;
using Orleans.Providers;
using Orleans.Streams;

namespace Grains
{
    [StorageProvider(ProviderName = "mainStorage")]
    public class UserGrain : Grain<UserGrainState>, IUserGrain
    {
        public Task<string> SayHello(string greeting)
        {
            //State.Events.Add(greeting);
            //base.WriteStateAsync();
           return Task.FromResult(greeting);
            
        }

        public override async Task OnActivateAsync()
        {
            var kafkaProvider = GetStreamProvider("KafkaStreamProvider");
            var testStream = kafkaProvider.GetStream<string>(Guid.Empty, "testTopic"); // todo: use stream utils
            //To resume stream in case of stream deactivation
            var subscriptionHandles = await testStream.GetAllSubscriptionHandles();

            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                {
                    await subscriptionHandle.ResumeAsync(OnNextTestMessage);
                }
            }

            await testStream.SubscribeAsync(OnNextTestMessage);
        }

        private Task OnNextTestMessage(string message, StreamSequenceToken sequenceToken)
        {
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Event value: {message}");
            return Task.CompletedTask;
        }
    }
}
