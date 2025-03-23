using FuncFlow.Tests.Helpers;

namespace FuncFlow.Tests;

public class FuncPipelineTests
{
    // Simple context class for testing
    public class TestContext
    {
        public int Value { get; set; }
        public List<string> ExecutionSteps { get; } = [];
    }

    // Simple service for dependency injection testing
    public class TestService
    {
        public int Multiplier { get; set; } = 10;
    }

    
    
    private Type[] NoDependencies => [];

    [Fact]
    public async Task ExecuteAsync_WithSynchronousSteps_ExecutesAllSteps()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step1");
                    ctx.Value += 1;
                    return ctx;
                }),
                NoDependencies,
                "Step1"),

            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step2");
                    ctx.Value += 2;
                    return ctx;
                }),
                NoDependencies,
                "Step2")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        Assert.Same(context, result);
        Assert.Equal(3, result.Value);
        Assert.Equal(new[] { "Step1", "Step2" }, result.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithAsynchronousSteps_ExecutesAllSteps()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, Task<TestContext>>(async ctx =>
                {
                    ctx.ExecutionSteps.Add("AsyncStep1");
                    ctx.Value += 1;
                    await Task.Delay(10);
                    return ctx;
                }),
                NoDependencies,
                "AsyncStep1"),

            (new Func<TestContext, Task<TestContext>>(async ctx =>
                {
                    ctx.ExecutionSteps.Add("AsyncStep2");
                    ctx.Value += 2;
                    await Task.Delay(10);
                    return ctx;
                }),
                NoDependencies,
                "AsyncStep2")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        Assert.Same(context, result);
        Assert.Equal(3, result.Value);
        Assert.Equal(new[] { "AsyncStep1", "AsyncStep2" }, result.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithMixedSteps_ExecutesAllSteps()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step1");
                    ctx.Value += 1;
                    return ctx;
                }),
                NoDependencies,
                "Step1"),

            (new Func<TestContext, Task<TestContext>>(async ctx =>
                {
                    ctx.ExecutionSteps.Add("AsyncStep");
                    ctx.Value += 2;
                    await Task.Delay(10);
                    return ctx;
                }),
                NoDependencies,
                "AsyncStep"),

            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step3");
                    ctx.Value += 3;
                    return ctx;
                }),
                NoDependencies,
                "Step3")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        Assert.Same(context, result);
        Assert.Equal(6, result.Value);
        Assert.Equal(new[] { "Step1", "AsyncStep", "Step3" }, result.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithDependencyInjection_ResolvesAndUsesDependencies()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var testService = new TestService { Multiplier = 5 };
        serviceProvider.AddService(testService);

        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, TestService, TestContext>((ctx, svc) =>
                {
                    ctx.ExecutionSteps.Add("DependencyStep");
                    ctx.Value *= svc.Multiplier;
                    return ctx;
                }),
                [typeof(TestService)],
                "DependencyStep")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 3 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        Assert.Same(context, result);
        Assert.Equal(15, result.Value); // 3 * 5
        Assert.Equal(new[] { "DependencyStep" }, result.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithPipelineError_StopsExecutionButReturnsContext()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step1");
                    ctx.Value += 1;
                    return ctx;
                }),
                NoDependencies,
                "Step1"),

            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("ErrorStep");
                    throw new PipelineError("Intentional error");
                }),
                NoDependencies,
                "ErrorStep"),

            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("ShouldNotExecute");
                    ctx.Value += 10;
                    return ctx;
                }),
                NoDependencies,
                "ShouldNotExecute")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        Assert.Same(context, result);
        Assert.Equal(1, result.Value); // Only the first step executed
        Assert.Equal(new[] { "Step1", "ErrorStep" }, result.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithGeneralException_ThrowsPipelineStepException()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("Step1");
                    ctx.Value += 1;
                    return ctx;
                }),
                NoDependencies,
                "Step1"),

            (new Func<TestContext, TestContext>(ctx =>
                {
                    ctx.ExecutionSteps.Add("ExceptionStep");
                    throw new InvalidOperationException("General exception");
                }),
                NoDependencies,
                "ExceptionStep")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PipelineStepException>(() =>
            pipeline.ExecuteAsync(context));

        Assert.Contains("Error executing pipeline step 1", exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Equal("General exception", exception.InnerException.Message);

        // Verify the context state before the exception
        Assert.Equal(1, context.Value);
        Assert.Equal(new[] { "Step1", "ExceptionStep" }, context.ExecutionSteps);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidReturnType_ThrowsInvalidOperationException()
    {
        // Arrange
        var serviceProvider = new TestServiceProvider();
        var steps = new List<(Delegate Function, Type[] DependencyTypes, string StepName)>
        {
            // This step returns string instead of TestContext
            (new Func<TestContext, string>(ctx =>
                {
                    ctx.ExecutionSteps.Add("InvalidReturnStep");
                    return "Invalid return type";
                }),
                NoDependencies,
                "InvalidReturnStep")
        };

        var pipeline = new FuncPipeline<TestContext>(serviceProvider, steps);
        var context = new TestContext { Value = 0 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PipelineStepException>(() =>
            pipeline.ExecuteAsync(context));

        Assert.Contains("Error executing pipeline step 0", exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Contains("returned an invalid result type", exception.InnerException.Message);
    }
}
