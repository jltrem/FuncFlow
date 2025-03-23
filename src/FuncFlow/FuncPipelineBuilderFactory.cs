namespace FuncFlow;

public interface IFuncPipelineBuilderFactory
{
    IFuncPipelineBuilder<TContext> Create<TContext>();
}

public class FuncPipelineBuilderFactory(IServiceProvider provider) : IFuncPipelineBuilderFactory
{
    public IFuncPipelineBuilder<TContext> Create<TContext>() =>
        new FuncPipelineBuilder<TContext>(provider);
}
