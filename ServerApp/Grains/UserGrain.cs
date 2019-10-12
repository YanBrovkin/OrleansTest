using System.Threading.Tasks;
using Domain.Aggregates;
using Domain.Interfaces;
using Orleans;
using Orleans.Providers;

namespace ServerApp.Grains
{
    [StorageProvider(ProviderName = "mainStorage")]
    public class UserGrain : Grain<UserGrainState>, IUserGrain
    {
        public Task<string> SayHello(string greeting)
        {
            State.Events.Add(greeting);
            base.WriteStateAsync();
            return Task.FromResult(greeting);
        }
    }
}
