using Microsoft.Extensions.DependencyInjection;
using MixDiTest.Interfaces;
using MixDiTest.Services;

namespace MixDiTest.Services;

public class MixedServiceProvider : IServiceProvider, IServiceScopeFactory
{
    private readonly JabServiceContainer _jabContainer;
    private readonly IServiceProvider _msDiProvider;
    private readonly IServiceScopeFactory _msDiScopeFactory;

    public MixedServiceProvider(JabServiceContainer jabContainer, IServiceProvider msDiProvider)
    {
        _jabContainer = jabContainer;
        _msDiProvider = msDiProvider;
        // used for creating mixed scope
        _msDiScopeFactory = msDiProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public object? GetService(Type serviceType)
    {
        // Try Jab first
        try
        {
            // Use reflection to call the generic GetService method
            var getServiceMethod = typeof(JabServiceContainer).GetMethod("GetService", new[] { typeof(string) });
            if (getServiceMethod != null)
            {
                var genericMethod = getServiceMethod.MakeGenericMethod(serviceType);
                var jabService = genericMethod.Invoke(_jabContainer, new object[] { "" });
                if (jabService != null)
                {
                    return jabService;
                }
            }
        }
        catch
        {
            // Jab couldn't resolve the service, fallback to MS.DI
        }

        // Fallback to MS.DI
        return _msDiProvider.GetService(serviceType);
    }

    public IServiceScope CreateScope()
    {
        return new MixedServiceScope(_jabContainer, _msDiScopeFactory.CreateScope());
    }
}

public class MixedServiceScope : IServiceScope
{
    private readonly JabServiceContainer _jabContainer;
    private readonly IServiceScope _msDiScope;
    private readonly Dictionary<Type, object> _jabScopedInstances = new();

    public MixedServiceScope(JabServiceContainer jabContainer, IServiceScope msDiScope)
    {
        _jabContainer = jabContainer;
        _msDiScope = msDiScope;
        ServiceProvider = new ScopedMixedServiceProvider(_jabContainer, msDiScope.ServiceProvider, _jabScopedInstances);
    }

    public IServiceProvider ServiceProvider { get; }

    public void Dispose()
    {
        // Dispose Jab scoped instances
        foreach (var instance in _jabScopedInstances.Values)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _jabScopedInstances.Clear();

        // Dispose MS.DI scope
        _msDiScope?.Dispose();
    }
}

// Scoped service provider that properly handles Jab scoped services
public class ScopedMixedServiceProvider : IServiceProvider
{
    private readonly JabServiceContainer _jabContainer;
    private readonly IServiceProvider _msDiProvider;
    private readonly Dictionary<Type, object> _jabScopedInstances;

    public ScopedMixedServiceProvider(JabServiceContainer jabContainer, IServiceProvider msDiProvider, Dictionary<Type, object> jabScopedInstances)
    {
        _jabContainer = jabContainer;
        _msDiProvider = msDiProvider;
        _jabScopedInstances = jabScopedInstances;
    }

    public object? GetService(Type serviceType)
    {
        // Try Jab first
        try
        {
            var jabLifetime = GetJabServiceLifetime(serviceType);
            
            if (jabLifetime == ServiceLifetime.Scoped)
            {
                // Return existing instance if already created in this scope
                if (_jabScopedInstances.TryGetValue(serviceType, out var existingInstance))
                {
                    return existingInstance;
                }

                // Create new instance and store it for this scope
                var jabService = CreateJabService(serviceType);
                if (jabService != null)
                {
                    _jabScopedInstances[serviceType] = jabService;
                    return jabService;
                }
            }
            else if (jabLifetime == ServiceLifetime.Singleton || jabLifetime == ServiceLifetime.Transient)
            {
                // For singleton and transient services, use Jab directly
                var jabService = CreateJabService(serviceType);
                if (jabService != null)
                {
                    return jabService;
                }
            }
        }
        catch
        {
            // Jab couldn't resolve the service, fallback to MS.DI
        }

        // Fallback to MS.DI
        return _msDiProvider.GetService(serviceType);
    }

    private ServiceLifetime GetJabServiceLifetime(Type serviceType)
    {
        // Determine lifetime using reflection (no caching for simplicity)
        if (IsServiceRegisteredWithLifetime(serviceType, "Singleton"))
        {
            return ServiceLifetime.Singleton;
        }
        else if (IsServiceRegisteredWithLifetime(serviceType, "Scoped"))
        {
            return ServiceLifetime.Scoped;
        }
        else if (IsServiceRegisteredWithLifetime(serviceType, "Transient"))
        {
            return ServiceLifetime.Transient;
        }

        return ServiceLifetime.Transient; // Default fallback
    }

    private object? CreateJabService(Type serviceType)
    {
        try
        {
            var getServiceMethod = typeof(JabServiceContainer).GetMethod("GetService", new[] { typeof(string) });
            if (getServiceMethod != null)
            {
                var genericMethod = getServiceMethod.MakeGenericMethod(serviceType);
                return genericMethod.Invoke(_jabContainer, new object[] { "" });
            }
        }
        catch
        {
            // Jab couldn't create the service
        }
        return null;
    }

    private bool IsServiceRegisteredWithLifetime(Type serviceType, string lifetime)
    {
        try
        {
            // Get the JabServiceContainer type
            var containerType = typeof(JabServiceContainer);
            
            // Get all custom attributes on the container
            var attributes = containerType.GetCustomAttributes(false);
            
            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();
                
                // Check if this is a Jab lifetime attribute (Transient<T>, Scoped<T>, Singleton<T>)
                if (attributeType.Name.StartsWith(lifetime) && attributeType.IsGenericType)
                {
                    var genericArgs = attributeType.GetGenericArguments();
                    if (genericArgs.Length >= 1 && genericArgs[0] == serviceType)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        catch
        {
            // If reflection fails, fall back to MS.DI
            return false;
        }
    }
}
