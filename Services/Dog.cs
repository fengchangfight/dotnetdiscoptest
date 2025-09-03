using MixDiTest.Interfaces;

namespace MixDiTest.Services;

public class Dog : IAnimal
{
    public string MakeSound()
    {
        return "Woof!";
    }

    public string GetName()
    {
        return "Dog";
    }
}
