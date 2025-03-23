using FuncFlow.Tests.Helpers;


using System.Reflection;


namespace FuncFlow.Tests;

public class FuncPipelineBuilderTests
{
    // Simple context class for testing
    public class TestContext
    {
        public int Value { get; set; }
        public List<string> ExecutionSteps { get; } = new();
    }

    // Simple services for dependency injection testing
    public class ServiceOne
    {
        public int Value => 1;
    }

    public class ServiceTwo
    {
        public int Value => 2;
    }

    public class ServiceThree
    {
        public int Value => 3;
    }

    // Simple service provider for testing
    private static IServiceProvider NewServiceProvider()
    {
        var services = new TestServiceProvider();
        services.AddService(new ServiceOne());
        services.AddService(new ServiceTwo());
        services.AddService(new ServiceThree());
        return services;
    }

    [Fact]
    public void AddStep_WithNoParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFunc<TestContext> stepFunc = ctx =>
        {
            ctx.Value++;
            return ctx;
        };

        // Act
        builder.AddStep(stepFunc, "TestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Empty(steps[0].DependencyTypes);
        Assert.Equal("TestStep", steps[0].StepName);
    }

    [Fact]
    public void AddStep_WithOneParameter_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFunc<TestContext, ServiceOne> stepFunc = (ctx, svc) =>
        {
            ctx.Value += svc.Value;
            return ctx;
        };

        // Act
        builder.AddStep(stepFunc, "TestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Single(steps[0].DependencyTypes);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal("TestStep", steps[0].StepName);
    }

    [Fact]
    public void AddStep_WithTwoParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFunc<TestContext, ServiceOne, ServiceTwo> stepFunc = (ctx, svc1, svc2) =>
        {
            ctx.Value += svc1.Value + svc2.Value;
            return ctx;
        };

        // Act
        builder.AddStep(stepFunc, "TestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Equal(2, steps[0].DependencyTypes.Length);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal(typeof(ServiceTwo), steps[0].DependencyTypes[1]);
        Assert.Equal("TestStep", steps[0].StepName);
    }

    [Fact]
    public void AddStep_WithThreeParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFunc<TestContext, ServiceOne, ServiceTwo, ServiceThree> stepFunc = (ctx, svc1, svc2, svc3) =>
        {
            ctx.Value += svc1.Value + svc2.Value + svc3.Value;
            return ctx;
        };

        // Act
        builder.AddStep(stepFunc, "TestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Equal(3, steps[0].DependencyTypes.Length);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal(typeof(ServiceTwo), steps[0].DependencyTypes[1]);
        Assert.Equal(typeof(ServiceThree), steps[0].DependencyTypes[2]);
        Assert.Equal("TestStep", steps[0].StepName);
    }

    [Fact]
    public void AddAsyncStep_WithNoParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFuncAsync<TestContext> stepFunc = async ctx =>
        {
            await Task.Delay(1);
            ctx.Value++;
            return ctx;
        };

        // Act
        builder.AddAsyncStep(stepFunc, "AsyncTestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Empty(steps[0].DependencyTypes);
        Assert.Equal("AsyncTestStep", steps[0].StepName);
    }

    [Fact]
    public void AddAsyncStep_WithOneParameter_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFuncAsync<TestContext, ServiceOne> stepFunc = async (ctx, svc) =>
        {
            await Task.Delay(1);
            ctx.Value += svc.Value;
            return ctx;
        };

        // Act
        builder.AddAsyncStep(stepFunc, "AsyncTestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Single(steps[0].DependencyTypes);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal("AsyncTestStep", steps[0].StepName);
    }

    [Fact]
    public void AddAsyncStep_WithTwoParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFuncAsync<TestContext, ServiceOne, ServiceTwo> stepFunc = async (ctx, svc1, svc2) =>
        {
            await Task.Delay(1);
            ctx.Value += svc1.Value + svc2.Value;
            return ctx;
        };

        // Act
        builder.AddAsyncStep(stepFunc, "AsyncTestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Equal(2, steps[0].DependencyTypes.Length);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal(typeof(ServiceTwo), steps[0].DependencyTypes[1]);
        Assert.Equal("AsyncTestStep", steps[0].StepName);
    }

    [Fact]
    public void AddAsyncStep_WithThreeParameters_AddsStepCorrectly()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);
        StepFuncAsync<TestContext, ServiceOne, ServiceTwo, ServiceThree> stepFunc = async (ctx, svc1, svc2, svc3) =>
        {
            await Task.Delay(1);
            ctx.Value += svc1.Value + svc2.Value + svc3.Value;
            return ctx;
        };

        // Act
        builder.AddAsyncStep(stepFunc, "AsyncTestStep");

        // Get private field value for inspection using reflection
        var stepsField = builder.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (List<(Delegate Function, Type[] DependencyTypes, string StepName)>)stepsField.GetValue(builder);

        // Assert
        Assert.Single(steps);
        Assert.Equal(stepFunc, steps[0].Function);
        Assert.Equal(3, steps[0].DependencyTypes.Length);
        Assert.Equal(typeof(ServiceOne), steps[0].DependencyTypes[0]);
        Assert.Equal(typeof(ServiceTwo), steps[0].DependencyTypes[1]);
        Assert.Equal(typeof(ServiceThree), steps[0].DependencyTypes[2]);
        Assert.Equal("AsyncTestStep", steps[0].StepName);
    }

    [Fact]
    public void Build_CreatesPipelineWithCorrectSteps()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);

        StepFunc<TestContext> step1 = ctx =>
        {
            ctx.ExecutionSteps.Add("Step1");
            return ctx;
        };

        StepFuncAsync<TestContext, ServiceOne> step2 = async (ctx, svc) =>
        {
            await Task.Delay(1);
            ctx.ExecutionSteps.Add("Step2");
            ctx.Value += svc.Value;
            return ctx;
        };

        builder.AddStep(step1, "Step1")
            .AddAsyncStep(step2, "Step2");

        // Act
        var pipeline = builder.Build();

        // Assert
        Assert.NotNull(pipeline);

        // Test that the pipeline works as expected
        var context = new TestContext { Value = 0 };
        var result = pipeline.ExecuteAsync(context).GetAwaiter().GetResult();

        Assert.Equal(1, result.Value); // ServiceOne.Value = 1
        Assert.Equal(new[] { "Step1", "Step2" }, result.ExecutionSteps);
    }

    [Fact]
    public void AddMultipleSteps_MaintainsOrder()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);

        // Act
        builder.AddStep(ctx =>
            {
                ctx.ExecutionSteps.Add("Step1");
                return ctx;
            }, "Step1")
            .AddStep(ctx =>
            {
                ctx.ExecutionSteps.Add("Step2");
                return ctx;
            }, "Step2")
            .AddStep(ctx =>
            {
                ctx.ExecutionSteps.Add("Step3");
                return ctx;
            }, "Step3");

        var pipeline = builder.Build();
        var context = new TestContext();

        // Assert
        var result = pipeline.ExecuteAsync(context).GetAwaiter().GetResult();
        Assert.Equal(new[] { "Step1", "Step2", "Step3" }, result.ExecutionSteps);
    }

    [Fact]
    public void ChainedMethodCalls_ReturnsSameBuilderInstance()
    {
        // Arrange
        var serviceProvider = NewServiceProvider();
        var builder = new FuncPipelineBuilder<TestContext>(serviceProvider);

        // Act
        var builder1 = builder.AddStep(ctx => ctx);
        var builder2 = builder1.AddStep(ctx => ctx);

        // Assert
        Assert.Same(builder, builder1);
        Assert.Same(builder1, builder2);
    }
}
