using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class Book : IPaper
{
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
}
