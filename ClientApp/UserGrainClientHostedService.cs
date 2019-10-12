using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
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
            // example of calling grains from the initialized client
            for (var i = 0; i <= 200; i++)
            {
                var msg = $"Message{i}";
                var user = this.client.GetGrain<IUserGrain>(i);
                var response = await user.SayHello(msg);
                Console.WriteLine(response);
            }
            Console.ReadLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
