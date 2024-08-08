using github_action;
using Microsoft.EntityFrameworkCore;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.Decorate<IMemberRepository, CachedMemberRepository>();

builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
    string connection = builder.Configuration
        .GetConnectionString("Redis")!;

    redisOptions.Configuration = connection;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    (sp, optionsBuilder) =>
    {
        optionsBuilder.UseInMemoryDatabase(nameof(ApplicationDbContext));
    });

builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/member", (MemberRequest memberRequest, IMemberRepository repository) =>
    {

        var member = Member.Create(Guid.NewGuid(), memberRequest.Email, memberRequest.FirstName, memberRequest.LastName);

        repository.Add(member);
        return Results.Ok(member);
    })
.WithName("GetWeatherForecast")
.WithOpenApi();



app.MapGet("/member/{id}", async (Guid id, IMemberRepository repository) =>
    {
        var member = await repository.GetByIdAsync(id);

        if (member is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(member);
    })
    .WithName("GetWeatherForecastGet")
    .WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class MemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
