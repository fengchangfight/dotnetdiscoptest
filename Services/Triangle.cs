using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class Triangle : IShape
{
    private readonly double _base = 10.0;
    private readonly double _height = 8.0;

    public string GetName()
    {
        return "Triangle";
    }

    public int GetSides()
    {
        return 3;
    }

    public double GetArea()
    {
        return 0.5 * _base * _height;
    }
}
