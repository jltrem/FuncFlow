using Microsoft.Extensions.Logging;

namespace simple_func_pipeline;

public class App
{
    private readonly ILogger _logger;
    private readonly MyPipeline _pipeline;

    public App(ILogger<App> logger, MyPipeline pipeline)
    {
        _logger = logger;
        _pipeline = pipeline;
    }
    public async Task RunAsync()
    { 
        await _pipeline.RunAsync();
    }
}

