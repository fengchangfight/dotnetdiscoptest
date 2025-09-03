using Jab;
using MixDiTest.Interfaces;
using MixDiTest.Services;

namespace MixDiTest.Services;

[ServiceProvider]
[Transient<IAnimal, Dog>]
[Scoped<IPaper, Book>]
[Singleton<IShape, Triangle>]
public partial class JabServiceContainer
{
}
