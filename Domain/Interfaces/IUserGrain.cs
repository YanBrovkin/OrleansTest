using System.Threading.Tasks;
using Orleans;

namespace Domain.Interfaces
{
    public interface IUserGrain: IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
