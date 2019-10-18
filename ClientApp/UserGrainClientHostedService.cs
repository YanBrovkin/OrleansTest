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
            //var streamProvider = client.GetStreamProvider("KafkaStreamProvider");
            //var stream = streamProvider.GetStream<SimpleResultSpecification>(Guid.Empty, "testTopic");
            

            // example of calling grains from the initialized client
            for (var i = 0; i <= 200; i++)
            {
                var user = this.client.GetGrain<IUserGrain>(i);
                var result = await user.SayHello($"Message{i}");
                //await stream.OnNextAsync(new SimpleResultSpecification
                //{
                //    Value = i
                //});
                //
                Console.WriteLine(result);
            }
            Console.ReadLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
