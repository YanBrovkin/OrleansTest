using System.Net;
using System.Threading.Tasks;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using ServerApp.Configuration;

namespace ConsoleApp
{
    class Program
    {
        static Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseLocalhostClustering()
                        .AddAdoNetGrainStorage("mainStorage", options =>
                        {
                            options.Invariant = "System.Data.SqlClient";
                            options.ConnectionString = "Data Source=Y-BROVKIN-M\\SQL2016;Initial Catalog=akkatest;Integrated Security=True";
                            options.UseJsonFormat = true;
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "ServerApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UserGrain).Assembly).WithReferences())
                        .AddKafkaConfig();
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .RunConsoleAsync();
            
            return builder;
        }
    }
}
