using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class ThirdPartyFoo : IFoo
{
    public string GetName() => "ThirdPartyFoo";
    
    public void DoSomething()
    {
        Console.WriteLine("ThirdPartyFoo is doing something");
    }
}
