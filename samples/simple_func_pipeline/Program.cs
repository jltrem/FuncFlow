using FuncFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using simple_func_pipeline.ChuckNorris;

namespace simple_func_pipeline;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                    .AddChuckNorrisService();
                
                services
                    .AddScoped<IFuncPipelineBuilder, FuncPipelineBuilder>()
                    .AddTransient<MyPipeline>()
                    .AddTransient<App>(); 
            })
            .Build();

        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync();
    }
}

