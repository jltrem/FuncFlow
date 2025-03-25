namespace FuncFlow;

public interface IFuncPipelineBuilder<TContext>
{
    IFuncPipelineBuilder<TContext> AddStep(string name, StepFunc<TContext> function);
    IFuncPipelineBuilder<TContext> AddStep<T1>(string name, StepFunc<TContext, T1> function);
    IFuncPipelineBuilder<TContext> AddStep<T1, T2>(string name, StepFunc<TContext, T1, T2> function);
    IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(string name, StepFunc<TContext, T1, T2, T3> function);
    IFuncPipelineBuilder<TContext> AddStep(string name, StepFuncAsync<TContext> function);
    IFuncPipelineBuilder<TContext> AddStep<T1>(string name, StepFuncAsync<TContext, T1> function);
    IFuncPipelineBuilder<TContext> AddStep<T1, T2>(string name, StepFuncAsync<TContext, T1, T2> function);
    IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(string name, StepFuncAsync<TContext, T1, T2, T3> function);
    IFuncPipeline<TContext> Build();
}

internal class FuncPipelineBuilder<TContext> : IFuncPipelineBuilder<TContext>
{
    private readonly IServiceProvider _provider;
    private readonly List<FuncPipelineStep<TContext>> _steps = new();
    private bool _built;
    
    public FuncPipelineBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }

    private IFuncPipelineBuilder<TContext> AddStep(FuncPipelineStep<TContext> step)
    {
        if (_built) throw new InvalidOperationException("FuncPipeline already built. Cannot add steps.");
        if (step is null) throw new ArgumentNullException(nameof(step));
        
        _steps.Add(step);
        return this;
    }
    
    public IFuncPipelineBuilder<TContext> AddStep(string name, StepFunc<TContext> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [], name));

    public IFuncPipelineBuilder<TContext> AddStep<T1>(string name, StepFunc<TContext, T1> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1)], name));
    
    public IFuncPipelineBuilder<TContext> AddStep<T1, T2>(string name, StepFunc<TContext, T1, T2> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1), typeof(T2)], name));

    public IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(string name, StepFunc<TContext, T1, T2, T3> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1), typeof(T2), typeof(T3)], name));

    public IFuncPipelineBuilder<TContext> AddStep(string name, StepFuncAsync<TContext> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [], name));

    public IFuncPipelineBuilder<TContext> AddStep<T1>(string name, StepFuncAsync<TContext, T1> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1)], name));

    public IFuncPipelineBuilder<TContext> AddStep<T1, T2>(string name, StepFuncAsync<TContext, T1, T2> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1), typeof(T2)], name));

    public IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(string name, StepFuncAsync<TContext, T1, T2, T3> function) =>
        AddStep(new FuncPipelineStep<TContext>(function, [typeof(T1), typeof(T2), typeof(T3)], name));

    
    public IFuncPipeline<TContext> Build()
    {
        if (_built) throw new InvalidOperationException("FuncPipeline already built.");
        _built = true;
        
        return new FuncPipeline<TContext>(_provider, _steps);
    }
}

public interface IFuncPipelineBuilder
{
    IFuncPipelineBuilder<TContext> WithContext<TContext>();
}

public class FuncPipelineBuilder(IServiceProvider provider) : IFuncPipelineBuilder
{
    public IFuncPipelineBuilder<TContext> WithContext<TContext>() =>
        new FuncPipelineBuilder<TContext>(provider);
}
