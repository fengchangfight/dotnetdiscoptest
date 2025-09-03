using Jab;
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
