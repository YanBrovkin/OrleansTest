using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Specifications;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace ClientApp
{
    public class UserGrainClientHostedService : IHostedService
    {
        private readonly IClusterClient client;

        public UserGrainClientHostedService(IClusterClient client, IHostApplicationLifetime lifetime)
        {
            this.client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var user = this.client.GetGrain<IUserGrain>(1);
            await user.SayHello($"Message");

            var streamProvider = client.GetStreamProvider("KafkaStreamProvider");
            var stream = streamProvider.GetStream<string>(Guid.Empty, "testTopic");
            Console.WriteLine("Ready to send messages ?");
            Console.ReadLine();
            //var user = this.client.GetGrain<IUserGrain>(1);
            // example of calling grains from the initialized client
            for (var i = 0; i <= 200; i++)
            {
                //var result = await user.SayHello($"Message{i}");
                //await stream.OnNextAsync(new SimpleResultSpecification
                //{
                //    Value = i
                //});
                //
                await stream.OnNextAsync($"Message: {i}");
                //Console.WriteLine(result);
            }
            Console.ReadLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
