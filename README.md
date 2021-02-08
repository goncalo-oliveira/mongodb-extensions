# MongoDB extensions for .NET

This repository contains MongoDB service extensions with dependency injection for .NET.

## Installing

Add a package reference from NuGet

```
dotnet add package Faactory.Extensions.MongoDB
```

## Usage

Just configure the DI container with the service extensions.

```csharp
public void ConfigureServices( IServiceCollection services )
{
    ...

    // add your services here.
    services.AddMongoService( options =>
    {
        options.Connection = "mongodb-connection-string";
    } );
}
```

And then you can inject the `IMongoService` wherever you need it.

```csharp
public class WeatherController : ControllerBase
{
    public WeatherController( IMongoService mongoService )
    {
        IMongoClient client = mongoService.Client;
    }
}
```
