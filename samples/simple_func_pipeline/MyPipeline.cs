using FuncFlow;
using Microsoft.Extensions.Logging;
using simple_func_pipeline.ChuckNorris;

namespace simple_func_pipeline;

record MyPipelineContext
{
    public string? JokeText { get; set; }
    public int Length { get; set; }
}

public class MyPipeline
{
    private readonly ILogger _logger;
    private readonly IFuncPipeline<MyPipelineContext> _pipeline;

    public MyPipeline(ILogger<MyPipeline> logger, IFuncPipelineBuilder builder)
    {
        _logger = logger;

        _pipeline = builder
            .WithContext<MyPipelineContext>()
            .AddStep(
                "fetch random joke",
                async (MyPipelineContext ctx, IChuckNorrisService chuckNorrisService) =>
                {
                    var joke = await chuckNorrisService.GetRandomJokeAsync();
                    ctx.JokeText = joke.Value;
                    return ctx;
                }
            )
            .AddStep(
                "get joke length", ctx =>
                {
                    ctx.Length = ctx.JokeText!.Length;
                    return ctx;
                }
            )
            .Build();
    }

    public async Task RunAsync()
    {
        var input = new MyPipelineContext();
        var output = await _pipeline.ExecuteAsync(input);
        _logger.LogInformation($"Joke length: {output.Length}");
    }

}
