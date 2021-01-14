using System;
using System.Net.Http;
using System.Threading.Tasks;
using Location.IpGeolocation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Weather.Abstractions;
using Weather.Weatherstack;

namespace Weather.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    var locationConfigSection = hostingContext.Configuration.GetSection("ipgeolocation");
                    var locationConfig = locationConfigSection.Get<IpGeolocationConfig>();
                    services.Configure<IpGeolocationConfig>(locationConfigSection);

                    services.AddHttpClient<ILocationGetter, LocationGetter>().AddPolicyHandler(GetRetryPolicy());

                    var weatherConfigSection = hostingContext.Configuration.GetSection("weatherstack");
                    var weatherConfig = locationConfigSection.Get<WeatherstackConfig>();
                    services.Configure<WeatherstackConfig>(weatherConfigSection);

                    services.AddHttpClient<IWeatherGetter, WeatherGetter>().AddPolicyHandler(GetRetryPolicy());

                    services.AddTransient<IWeatherService, WeatherService>();
                });

            var host = hostBuilder.Build();

            var exit = false;

            do
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Type the location, for local weather just press enter");


                var input = System.Console.ReadLine();

                using (var serviceScope = host.Services.CreateScope())
                {
                    try
                    {
                        var weatherService = serviceScope.ServiceProvider.GetRequiredService<IWeatherService>();
                        var result = await weatherService.GetWeatherByLocation(input);

                        if (result.TempCelsius.HasValue)
                        {
                            System.Console.WriteLine($"Location: {result.Location}");
                            System.Console.WriteLine($"Temperature: {result.TempCelsius} Celsius, {result.Description}.");
                        }
                        else
                        {
                            System.Console.WriteLine("not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Error. " + ex.ToString());
                    }
                }
            }
            while (!exit);
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
