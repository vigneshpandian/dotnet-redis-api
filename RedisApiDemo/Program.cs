using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string redisConnectionString = $"{builder.Configuration.GetValue<string>("Redis:Host")},password={builder.Configuration.GetValue<string>("Redis:Password")}";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });
builder.Services.AddHealthChecks().AddRedis(redisConnectionString, name: "redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    

    return ConnectionMultiplexer.Connect(redisConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready")
});

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = check => check.Name == "redis",
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    },
});


app.Run();
