using Microsoft.AspNetCore.Mvc;
using MixDiTest.Interfaces;

namespace MixDiTest.Controllers;

[ApiController]
[Route("[controller]")]
public class ScopeTestController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ScopeTestController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpGet("scoped-instance-test")]
    public IActionResult TestScopedInstances()
    {
        var results = new List<object>();

        // Test 1: Get IPaper instances within the same request scope
        using (var scope1 = _serviceProvider.CreateScope())
        {
            var paper1 = scope1.ServiceProvider.GetService<IPaper>();
            var paper2 = scope1.ServiceProvider.GetService<IPaper>();
            
            results.Add(new
            {
                Test = "Same Scope - IPaper instances",
                Instance1 = paper1?.GetHashCode(),
                Instance2 = paper2?.GetHashCode(),
                AreSame = ReferenceEquals(paper1, paper2),
                Expected = true,
                Passed = ReferenceEquals(paper1, paper2)
            });
        }

        // Test 2: Get IPaper instances from different scopes
        IPaper paperFromScope1;
        IPaper paperFromScope2;
        
        using (var scope1 = _serviceProvider.CreateScope())
        {
            paperFromScope1 = scope1.ServiceProvider.GetService<IPaper>();
        }

        using (var scope2 = _serviceProvider.CreateScope())
        {
            paperFromScope2 = scope2.ServiceProvider.GetService<IPaper>();
        }

        results.Add(new
        {
            Test = "Different Scopes - IPaper instances",
            Instance1 = paperFromScope1?.GetHashCode(),
            Instance2 = paperFromScope2?.GetHashCode(),
            AreSame = ReferenceEquals(paperFromScope1, paperFromScope2),
            Expected = false,
            Passed = !ReferenceEquals(paperFromScope1, paperFromScope2)
        });

        // Test 3: Test transient services (should be different instances)
        using (var scope1 = _serviceProvider.CreateScope())
        {
            var animal1 = scope1.ServiceProvider.GetService<IAnimal>();
            var animal2 = scope1.ServiceProvider.GetService<IAnimal>();
            
            results.Add(new
            {
                Test = "Same Scope - IAnimal instances (Transient)",
                Instance1 = animal1?.GetHashCode(),
                Instance2 = animal2?.GetHashCode(),
                AreSame = ReferenceEquals(animal1, animal2),
                Expected = false,
                Passed = !ReferenceEquals(animal1, animal2)
            });
        }

        // Test 4: Test singleton services (should be same instances)
        using (var scope1 = _serviceProvider.CreateScope())
        {
            var shape1 = scope1.ServiceProvider.GetService<IShape>();
            var shape2 = scope1.ServiceProvider.GetService<IShape>();
            
            results.Add(new
            {
                Test = "Same Scope - IShape instances (Singleton)",
                Instance1 = shape1?.GetHashCode(),
                Instance2 = shape2?.GetHashCode(),
                AreSame = ReferenceEquals(shape1, shape2),
                Expected = true,
                Passed = ReferenceEquals(shape1, shape2)
            });
        }

        return Ok(new
        {
            Message = "Scoping Test Results",
            Results = results,
            AllTestsPassed = results.All(r => (bool)r.GetType().GetProperty("Passed")!.GetValue(r)!)
        });
    }
}
