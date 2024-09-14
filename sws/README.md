# SWKOM_project
 Softwarekomponentensysteme Labor semester project


https://github.com/nataliacuevas/SWKOM_project

- Install the extension https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp

## Create a new project

- Check help for further information:
```shell
dotnet new webapi –help
```

- Create a new project in the subfolder sws:

```shell
dotnet new webapi --use-controllers -o sws
```

The default project structure:
- *sws/controllers*:
Contains all the endpoints which are reachable via HTTP. In this Example: GetWeatherForecast
- *Properties/launchSettings.json*:
Contains the startup information for the IDE
- *appsettings.json*:
Contains configuration information about the service
- *Program.cs*:
The starting point

Optional: Trust certificates to enable use of HTTPS:
```shell
dotnet dev-certs https --trust
```

## The first start
```shell
dotnet run --launch-profile https
```

https://localhost:7127/swagger/index.html

## Create a DAL

- Add NuGet package for Entity Framework Core:
```shell
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

- Add new Folder "Models"

- Add new Class "TodoItem.cs"
```csharp
namespace sws.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
```

- Add new Class "TodoContext.cs"
```csharp
using Microsoft.EntityFrameworkCore;

namespace sws.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } = null;
    }
}
```

- Register the context in the Program.cs
```csharp
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
```

## Create a new Controller

- Add required NuGet packages as pre-requisites:
```shell
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

- Add and update the codegenerator-tool:
```shell
dotnet tool uninstall -g dotnet-aspnet-codegenerator
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet tool update -g dotnet-aspnet-codegenerator
```

- Add a new Controller:
Here it is the TodoItemsController.cs using the TodoItem model and the TodoContext generated in the Controllers subfolder.
```shell
dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers
```

- Investigate Controllers\TodoItemsController

    - In line 12 you see the name of the controller. It will use the convention placeholder (class name minus Controller) – so it‘s TodoItems

      - For a full deep dive go to https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-8.0 

    - In line 18 the DbContext is injected

      - For DI deep dive go to https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0 

    - In line 25 you see the action for getting all items

    - In line 32 for a specific item

    - In line 47 we see the action for updating an item

    - In line 78 we see the action for add 
    
    - in line 88 for delete

## Run the example

https://localhost:7282/swagger/index.html

You should no be able to:
- Add a new item via POST
- GET the item
- Update the item via PUT
- And delete the item via DELETE

See [sws.http](sws.http)

- Try it! 

## Add a Logging provider

- Register a logging provider in the Program.cs

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
```

- Inject the logging instance in the TodoItemsController.cs

```csharp
        private readonly ILogger _logger;

        public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }
```

- We use structured logging, for a deep dive see https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0#log-message-template
- It‘s not printed well in the console – but in a real log provider you can search for key and value – that's a huge improvement because you don‘t have to parse strings

## Configuration

- Normally configuration is loaded on startup (e.g. Azure App Configuration) – but you can also provide it via files, if you want less complexity.
Deep dive https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0

- *appsettings.json* vs *launchSettings.json*:

    - launchSettings.json are just for the IDE (“Profiles”)

    - appsettings.json are the “real” application settings

    - See also https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0

## Dependency Injection

1. Register Dependency in Program.cs: builder.Services.AddScoped<IMyAdapter, MyAdapter>();
  
    - Define the scope – how long should the instance live?

2. Inject Dependency  public WeatherForecastController(ILogger<WeatherForecastController> logger, IMyAdapter myAdapter)

3. See also https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0 

## Important Links
- https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-8.0
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-8.0&tabs=windows 

