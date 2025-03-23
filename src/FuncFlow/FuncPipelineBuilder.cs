namespace FuncFlow;

public interface IFuncPipelineBuilder<TContext>
{
    IFuncPipelineBuilder<TContext> AddStep(StepFunc<TContext> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddStep<T1>(StepFunc<TContext, T1> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddStep<T1, T2>(StepFunc<TContext, T1, T2> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(StepFunc<TContext, T1, T2, T3> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddAsyncStep(StepFuncAsync<TContext> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddAsyncStep<T1>(StepFuncAsync<TContext, T1> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddAsyncStep<T1, T2>(StepFuncAsync<TContext, T1, T2> stepFunc, string stepName = "");
    IFuncPipelineBuilder<TContext> AddAsyncStep<T1, T2, T3>(StepFuncAsync<TContext, T1, T2, T3> stepFunc, string stepName = "");
}

public class FuncPipelineBuilder<TContext> : IFuncPipelineBuilder<TContext>
{
    private readonly IServiceProvider _provider;
    private readonly List<FuncPipelineStep> _steps = new();

    public FuncPipelineBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IFuncPipelineBuilder<TContext> AddStep(StepFunc<TContext> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddStep<T1>(StepFunc<TContext, T1> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1)], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddStep<T1, T2>(StepFunc<TContext, T1, T2> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1), typeof(T2)], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddStep<T1, T2, T3>(StepFunc<TContext, T1, T2, T3> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1), typeof(T2), typeof(T3)], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddAsyncStep(StepFuncAsync<TContext> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddAsyncStep<T1>(StepFuncAsync<TContext, T1> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1)], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddAsyncStep<T1, T2>(StepFuncAsync<TContext, T1, T2> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1), typeof(T2)], stepName));
        return this;
    }

    public IFuncPipelineBuilder<TContext> AddAsyncStep<T1, T2, T3>(StepFuncAsync<TContext, T1, T2, T3> stepFunc, string stepName = "")
    {
        _steps.Add(new FuncPipelineStep(stepFunc, [typeof(T1), typeof(T2), typeof(T3)], stepName));
        return this;
    }

    public FuncPipeline<TContext> Build()
    {
        return new FuncPipeline<TContext>(_provider, _steps);
    }
}

