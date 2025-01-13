using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB-CleverCloud"),
        new MySqlServerVersion(new Version(8, 0, 21))));

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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        options.RoutePrefix = string.Empty; // Optional: set Swagger UI to load at the root URL
    });
// }

// Map endpoints with more descriptive paths
app.MapGet("/todos", async (TodoService _todoService) =>
{
    var items = await _todoService.GetItems();  // Await asynchronous method
    return Results.Ok(items);  // Return the items asynchronously
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

app.Run();
