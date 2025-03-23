using Microsoft.Extensions.DependencyInjection;

namespace simple_func_pipeline.ChuckNorris;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChuckNorrisService(this IServiceCollection services)
    {
        services.AddHttpClient<IChuckNorrisService, ChuckNorrisService>(client =>
        {
            client.BaseAddress = new Uri("https://api.chucknorris.io/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
