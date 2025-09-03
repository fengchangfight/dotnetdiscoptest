using Microsoft.AspNetCore.Mvc;
using MixDiTest.Interfaces;

namespace MixDiTest.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IAnimal _animal;
    private readonly IMusic _music;
    private readonly IPaper _paper;
    private readonly IShape _shape;

    public TestController(IAnimal animal, IMusic music, IPaper paper, IShape shape)
    {
        _animal = animal;
        _music = music;
        _paper = paper;
        _shape = shape;
    }

    [HttpGet("animal")]
    public IActionResult GetAnimal()
    {
        return Ok(new
        {
            Name = _animal.GetName(),
            Sound = _animal.MakeSound(),
            Source = "Jab DI (Transient)"
        });
    }

    [HttpGet("music")]
    public IActionResult GetMusic()
    {
        return Ok(new
        {
            Genre = _music.GetGenre(),
            Play = _music.Play(),
            Source = "MS.DI (Transient)"
        });
    }

    [HttpGet("paper")]
    public IActionResult GetPaper()
    {
        return Ok(new
        {
            Title = _paper.GetTitle(),
            Content = _paper.GetContent(),
            PageCount = _paper.GetPageCount(),
            Source = "Jab DI (Scoped)"
        });
    }

    [HttpGet("shape")]
    public IActionResult GetShape()
    {
        return Ok(new
        {
            Name = _shape.GetName(),
            Sides = _shape.GetSides(),
            Area = _shape.GetArea(),
            Source = "Jab DI (Singleton)"
        });
    }

    [HttpGet("both")]
    public IActionResult GetBoth()
    {
        return Ok(new
        {
            Animal = new
            {
                Name = _animal.GetName(),
                Sound = _animal.MakeSound(),
                Source = "Jab DI (Transient)"
            },
            Music = new
            {
                Genre = _music.GetGenre(),
                Play = _music.Play(),
                Source = "MS.DI (Transient)"
            },
            Paper = new
            {
                Title = _paper.GetTitle(),
                Content = _paper.GetContent(),
                PageCount = _paper.GetPageCount(),
                Source = "Jab DI (Scoped)"
            },
            Shape = new
            {
                Name = _shape.GetName(),
                Sides = _shape.GetSides(),
                Area = _shape.GetArea(),
                Source = "Jab DI (Singleton)"
            }
        });
    }
}
