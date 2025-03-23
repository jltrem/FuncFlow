using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FuncFlow;

public interface IFuncPipeline<TContext>
{
    int StepCount { get; }
    string GetStepName(int stepIndex);

    Task<TContext> ExecuteStepAsync(TContext context, int stepIndex);
    
    Task<TContext> ExecuteAsync(TContext context);
    
    Task<TResult> ExecuteAsync<TResult, TStepError>(
        TContext context,
        Func<TContext, TResult> onStepsSuccess,
        Func<Exception, TContext, TResult> onStepFailed
    ) where TStepError : Exception;
}

public class FuncPipeline<TContext> : IFuncPipeline<TContext>
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;
    private readonly IReadOnlyList<FuncPipelineStep> _steps;

    public FuncPipeline(IServiceProvider provider, IEnumerable<FuncPipelineStep> steps)
    {
        _provider = provider;
        _steps = [..steps];

        _logger =
            provider.GetService(typeof(ILogger<FuncPipeline<TContext>>)) as ILogger<FuncPipeline<TContext>>
            ?? NullLogger<FuncPipeline<TContext>>.Instance;
    }
    
    public int StepCount => _steps.Count;

    public string GetStepName(int stepIndex) => _steps[stepIndex].Name;
    
    public async Task<TContext> ExecuteStepAsync(TContext context, int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= _steps.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(stepIndex));    
        }
        FuncPipelineStep step = _steps[stepIndex];
        
        var dependencies = step.DependencyTypes.Select(type => _provider.GetService(type)!).ToArray();

        var arguments = new object[dependencies.Length + 1];
        arguments[0] = context!;
        Array.Copy(dependencies, 0, arguments, 1, dependencies.Length);

        try
        {
            object? result = step.Function.DynamicInvoke(arguments);
        
            switch (result)
            {
                case Task<TContext> taskResult:
                    // This handles both true async methods and non-async Task-returning methods.
                    _logger.LogDebug("Step[{StepIndex}] `{StepName}` - awaiting Task", stepIndex, step.Name);
                    return await taskResult;
                
                case TContext directResult:
                    // Handle synchronous functions that return TContext directly
                    _logger.LogDebug("Step[{StepIndex}] `{StepName}` - synchronous", stepIndex, step.Name);
                    return directResult;
                
                default:
                    throw new InvalidOperationException($"Step[{stepIndex}] `{step.Name}` returned an invalid result type. Expected Task<{typeof(TContext).Name}> or {typeof(TContext).Name}, but got {result?.GetType().Name ?? "null"}.");
            }
        }
        catch (TargetInvocationException tie)
        {
            // Unwrap exceptions from synchronous methods
            if (tie.InnerException is not null)
            {
                throw tie.InnerException;
            }
            throw new InvalidOperationException("TargetInvocationException missing inner exception.", tie);
        }
    }
    
    public async Task<TContext> ExecuteAsync(TContext context)
    {
        for (int stepIndex = 0; stepIndex < _steps.Count; stepIndex++)
        {
            context = await ExecuteStepAsync(context, stepIndex);
        }
        return context;
    }

    public async Task<TResult> ExecuteAsync<TResult, TStepError>(TContext context, Func<TContext, TResult> onStepsSuccess, Func<Exception, TContext, TResult> onStepFailed) where TStepError : Exception
    {
        for (int stepIndex = 0; stepIndex < _steps.Count; stepIndex++) 
        {
            try
            {
                context = await ExecuteStepAsync(context, stepIndex);
            }
            catch (TStepError e)
            {
                return onStepFailed(e, context);
            }
        }

        return onStepsSuccess(context);
    }
}