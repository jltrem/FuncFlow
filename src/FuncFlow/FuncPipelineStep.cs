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

    public static FuncPipelineStep Create<TContext>(string stepName, StepFunc<TContext> stepFunc) =>
        new(stepFunc, [], stepName);

    public static FuncPipelineStep Create<TContext, T1>(string stepName, StepFunc<TContext, T1> stepFunc) =>
        new(stepFunc, [typeof(T1)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2>(string stepName, StepFunc<TContext, T1, T2> stepFunc) =>
        new(stepFunc, [typeof(T1), typeof(T2)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2, T3>(string stepName, StepFunc<TContext, T1, T2, T3> stepFunc) =>
        new(stepFunc, [typeof(T1), typeof(T2), typeof(T3)], stepName);

    public static FuncPipelineStep Create<TContext>(string stepName, StepFuncAsync<TContext> stepFunc) =>
        new(stepFunc, [], stepName);

    public static FuncPipelineStep Create<TContext, T1>(string stepName, StepFuncAsync<TContext, T1> stepFunc) =>
        new(stepFunc, [typeof(T1)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2>(string stepName, StepFuncAsync<TContext, T1, T2> stepFunc) =>
        new(stepFunc, [typeof(T1), typeof(T2)], stepName);

    public static FuncPipelineStep Create<TContext, T1, T2, T3>(string stepName, StepFuncAsync<TContext, T1, T2, T3> stepFunc) =>
        new(stepFunc, [typeof(T1), typeof(T2), typeof(T3)], stepName);
}
