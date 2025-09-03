using Jab;
using Microsoft.Extensions.DependencyInjection;
using MixDiTest.Interfaces;
using MixDiTest.Services;

namespace MixDiTest.Services;

[ServiceProvider]
[Transient<IAnimal, Dog>]
[Scoped<IPaper, Book>]
[Singleton<IShape, Triangle>]
public partial class JabServiceContainer : IDisposable
{
    private bool _disposed = false;
    private readonly IServiceProvider _externalServiceProvider;

    // Custom constructor to inject external service provider
    public JabServiceContainer(IServiceProvider externalServiceProvider = null)
    {
        _externalServiceProvider = externalServiceProvider;
    }

    // Override Jab's service resolution to use external service provider
    public T GetService<T>(string key = "")
    {
        // Try Jab's internal resolution first
        try
        {
            var baseMethod = typeof(JabServiceContainer).GetMethod("GetService", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, 
                null, new[] { typeof(string) }, null);
            
            if (baseMethod != null)
            {
                var genericMethod = baseMethod.MakeGenericMethod(typeof(T));
                var result = genericMethod.Invoke(this, new object[] { key });
                if (result != null)
                {
                    return (T)result;
                }
            }
        }
        catch
        {
            // Jab couldn't resolve, try external service provider
        }

        // Fallback to external service provider (MS.DI)
        if (_externalServiceProvider != null)
        {
            return _externalServiceProvider.GetService<T>();
        }

        return default(T);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here if needed
                // Jab will handle its own internal disposal
            }

            _disposed = true;
        }
    }

    // Finalizer (destructor) - safety net
    ~JabServiceContainer()
    {
        Dispose(false);
    }
}
