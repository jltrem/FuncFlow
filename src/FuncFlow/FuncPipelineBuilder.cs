namespace FuncFlow;

public interface IFuncPipelineBuilder
{
    IFuncPipelineBuilder AddStep(FuncPipelineStep step);
    IFuncPipeline<TContext> Build<TContext>();
}

public class FuncPipelineBuilder : IFuncPipelineBuilder
{
    private readonly IServiceProvider _provider;
    private readonly List<FuncPipelineStep> _steps = new();
    
    private bool _built;

    public FuncPipelineBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IFuncPipelineBuilder AddStep(FuncPipelineStep step)
    {
        if (_built) throw new InvalidOperationException("FuncPipeline already built. Cannot add steps.");
        if (step is null) throw new ArgumentNullException(nameof(step));
        
        _steps.Add(step);
        return this;
    }

    public IFuncPipeline<TContext> Build<TContext>()
    {
        if (_built) throw new InvalidOperationException("FuncPipeline already built.");
        _built = true;
        
        return new FuncPipeline<TContext>(_provider, _steps);
    }
}
