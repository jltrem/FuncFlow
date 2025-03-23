namespace FuncFlow.Tests.Helpers;

// Simple service provider implementation for testing
public class TestServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _services = new();

    public void AddService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public object? GetService(Type serviceType) =>
        _services.GetValueOrDefault(serviceType);
}
