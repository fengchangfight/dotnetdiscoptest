using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class Book : IPaper, IDisposable
{
    private readonly IFoo _foo;
    private readonly IBar _bar;
    
    // Constructor with dependencies - NO CHANGES NEEDED!
    public Book(IFoo foo, IBar bar)
    {
        _foo = foo;
        _bar = bar;
    }
    
    public string GetTitle()
    {
        return "The Great Adventure";
    }

    public string GetContent()
    {
        return "Once upon a time, in a land far away...";
    }

    public int GetPageCount()
    {
        return 250;
    }
    
    public void Dispose()
    {
        Console.WriteLine($"Book instance {GetHashCode()} disposed at {DateTime.Now}");
    }
}
