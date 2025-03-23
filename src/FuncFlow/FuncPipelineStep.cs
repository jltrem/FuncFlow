namespace FuncFlow;

public record FuncPipelineStep
{
    public Delegate Function { get; }
    public Type[] DependencyTypes { get; }
    public string Name { get; }

    private FuncPipelineStep(Delegate function, Type[] dependencyTypes, string name)
    {
        Function = function;
        DependencyTypes = dependencyTypes;
        Name = name;
    }

    public static FuncPipelineStep Create<TContext>(
        StepFuncAsync<TContext> stepFunc, string stepName = ""
    ) =>
        new(stepFunc, [], stepName);

    public static FuncPipelineStep Create<TContext, T1>(
        StepFuncAsync<TContext, T1> stepFunc, string stepName = ""
    ) =>
        new(stepFunc, [typeof(T1)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2>(
        StepFuncAsync<TContext, T1, T2> stepFunc, string stepName = ""
    ) =>
        new(stepFunc, [typeof(T1), typeof(T2)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2, T3>(
        StepFuncAsync<TContext, T1, T2, T3> stepFunc, string stepName = ""
    ) =>
        new(stepFunc, [typeof(T1), typeof(T2), typeof(T3)], stepName);

}
