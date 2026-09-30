using System.Text.Json.Serialization;
using WarriorForeverSim.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api");

api.MapGet("/options", SimulationRunner.GetOptions);

api.MapPost("/simulate", async (SimulateRequest request, CancellationToken cancellationToken) =>
{
    var errors = SimulationRunner.Validate(request);

    return errors.Count > 0
        ? Results.ValidationProblem(errors)
        : Results.Ok(await SimulationRunner.RunAsync(request, cancellationToken));
});

app.MapFallbackToFile("index.html");

app.Run();
