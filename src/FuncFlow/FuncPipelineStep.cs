using System.Runtime.InteropServices.ComTypes;

namespace FuncFlow;


public record FuncPipelineStep<TContext>
{
    public Delegate Function { get; }
    public Type ContextType => typeof(TContext);
    public Type[] DependencyTypes { get; }
    public string Name { get; }
    
    internal FuncPipelineStep(Delegate function, Type[] dependencyTypes, string name)
    {
        Function = function;
        DependencyTypes = dependencyTypes;
        Name = name;
    }
}
