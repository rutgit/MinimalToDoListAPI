using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

 var connectionString = builder.Configuration.GetConnectionString("DB_CONNECTION_STRING_REMOTE");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string is missing!");
    return;
}
try
{
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
catch (Exception ex)
{
    Console.WriteLine($"שגיאה: {ex.Message}");
}






// Retrieve the connection string from environment variables
// var connectionStringLocal = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_LOCAL")
//                            ?? throw new Exception("Local connection string not found!");

// var connectionStringRemote = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_REMOTE")
//                             ?? throw new Exception("Remote connection string not found!");

// Depending on the environment, use the appropriate connection string
//var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

//Console.WriteLine($"Local Connection String: {connectionStringLocal}");
// Console.WriteLine($"Remote Connection String: {connectionStringRemote}");
// Console.WriteLine($"Environment: {environment}");

// if (environment == "Development")
// {
//     builder.Services.AddDbContext<ToDoDbContext>(options =>
//         options.UseMySql(connectionStringLocal,
//             new MySqlServerVersion(new Version(8, 0, 21))));
// }
// else
// {
    // builder.Services.AddDbContext<ToDoDbContext>(options =>
    //     options.UseMySql(connectionStringRemote,
    //         new MySqlServerVersion(new Version(8, 0, 21))));
//}


builder.Services.AddScoped<TodoService>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer(); // For minimal APIs
builder.Services.AddSwaggerGen();  // Add Swagger generation

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()  // For local testing, allow all origins
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");

// Enable Swagger middleware
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI
    (options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        options.RoutePrefix = string.Empty; // Optional: set Swagger UI to load at the root URL
    });
// }

// Map endpoints with more descriptive paths
app.MapGet("/todos", async (TodoService _todoService) =>
{
     try
    {
        var items = await _todoService.GetItems();
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching todos: {ex.Message}");
        return Results.StatusCode(500);
    }
});

app.MapPost("/todos", async (TodoService _todoService, Item item) =>
{
    var result = await _todoService.PostItem(item);  // Await asynchronous method
    return Results.Ok(result);  // Return the result of posting the item asynchronously
});

app.MapPut("/todos/{id}", async (TodoService _todoService, int id, bool isComplete) =>
{
    var result = await _todoService.PutItem(id, isComplete);
    return Results.Ok(result);
});

app.MapDelete("/todos/{id}", async (TodoService _todoService, int id) =>
{
    var result = await _todoService.DeleteItem(id);  // Await asynchronous method
    return Results.Ok(result);  // Return the result of deleting the item asynchronously
});

app.MapGet("/",()=>"TodoList is running");

app.MapFallback(() => Results.NotFound("The requested endpoint does not exist."));

app.Run();
