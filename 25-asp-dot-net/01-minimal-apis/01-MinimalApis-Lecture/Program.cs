using BlogApi.Services;
using BlogApi.DTOs;
using BlogApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TimeService>();



var app = builder.Build();

var appName = builder.Configuration["AppName"] ?? "Default App";
var greeting = builder.Configuration["Greeting"] ?? "Hi";


app.Use(async (context, next) =>
{
  Console.WriteLine($"Handling request: {context.Request.Path}");
  // await next.Invoke();
  await next();
  Console.WriteLine($"Finished handling request.");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/config", () => $"{appName} says: {greeting}");
app.MapGet("/time", (TimeService ts) => ts.Now());


// User endpoints
app.MapUsers();

// Posts endpoints
app.MapGet("/posts", () => new[] { "Post 1", "Post 2" });
app.MapGet("/posts/{id}", (int id) => $"Post {id}");
app.MapPost("/posts", (PostRequestDto post) => $"Created post {post.Title}");
app.Run();
