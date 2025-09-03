using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class ThirdPartyBar : IBar
{
    public string GetValue() => "ThirdPartyBar Value";
    
    public void Process()
    {
        Console.WriteLine("ThirdPartyBar is processing");
    }
}
