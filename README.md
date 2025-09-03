# MixDiTest - Mixed Dependency Injection with Jab and MS.DI

This project demonstrates a mixed dependency injection setup using both Jab and Microsoft.Extensions.DependencyInjection (MS.DI) with fallback logic.

## Features

- **Jab DI**: Used for `IAnimal` interface with `Dog` implementation (Transient), `IPaper` interface with `Book` implementation (Scoped), and `IShape` interface with `Triangle` implementation (Singleton)
- **MS.DI**: Used for `IMusic` interface with `Jazz` implementation (Transient)
- **Fallback Logic**: Jab is tried first, then falls back to MS.DI if Jab cannot resolve the service
- **Consistent Scoping**: Both DI containers maintain consistent service lifetimes with proper scope management

## Project Structure

```
├── Controllers/
│   └── TestController.cs          # API controller demonstrating both DI systems
├── Interfaces/
│   ├── IAnimal.cs                 # Animal interface
│   ├── IMusic.cs                  # Music interface
│   ├── IPaper.cs                  # Paper interface
│   └── IShape.cs                  # Shape interface
├── Services/
│   ├── Dog.cs                     # Dog implementation (registered with Jab, Transient)
│   ├── Jazz.cs                    # Jazz implementation (registered with MS.DI, Transient)
│   ├── Book.cs                    # Book implementation (registered with Jab, Scoped)
│   ├── Triangle.cs                # Triangle implementation (registered with Jab, Singleton)
│   ├── JabServiceContainer.cs     # Jab service container
│   └── MixedServiceProvider.cs    # Combined service provider with fallback and scope management
├── Program.cs                     # Application entry point
├── Startup.cs                     # Application configuration
└── MixDiTest.csproj              # Project file
```

## How It Works

1. **Jab Registration**: 
   - `IAnimal` → `Dog` is registered with Transient lifetime in `JabServiceContainer.cs`
   - `IPaper` → `Book` is registered with Scoped lifetime in `JabServiceContainer.cs`
   - `IShape` → `Triangle` is registered with Singleton lifetime in `JabServiceContainer.cs`
2. **MS.DI Registration**: `IMusic` → `Jazz` is registered with Transient lifetime in `Startup.cs`
3. **Mixed Service Provider**: `MixedServiceProvider` tries Jab first, then falls back to MS.DI
4. **Consistent Scoping**: The mixed service provider implements `IServiceScopeFactory` to ensure proper scope management across both DI containers

## API Endpoints

- `GET /test/animal` - Returns animal info (from Jab DI, Transient)
- `GET /test/music` - Returns music info (from MS.DI, Transient)
- `GET /test/paper` - Returns paper info (from Jab DI, Scoped)
- `GET /test/shape` - Returns shape info (from Jab DI, Singleton)
- `GET /test/both` - Returns all services (animal, music, paper, and shape)

## Running the Project

```bash
dotnet run
```

The application will start on `https://localhost:5001` (or the configured port).

## Testing the DI

Visit the following URLs to test the mixed dependency injection:

- `https://localhost:5001/test/animal` - Should return Dog info from Jab (Transient)
- `https://localhost:5001/test/music` - Should return Jazz info from MS.DI (Transient)
- `https://localhost:5001/test/paper` - Should return Book info from Jab (Scoped)
- `https://localhost:5001/test/shape` - Should return Triangle info from Jab (Singleton)
- `https://localhost:5001/test/both` - Should return all four services with their respective DI sources
