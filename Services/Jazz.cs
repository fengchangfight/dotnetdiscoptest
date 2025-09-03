using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class Jazz : IMusic
{
    public string Play()
    {
        return "Playing smooth jazz...";
    }

    public string GetGenre()
    {
        return "Jazz";
    }
}
