using System.Net.Http.Headers;
using AiPortfolio.Api.Application.Interfaces;
using AiPortfolio.Api.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Load config.json alongside appsettings
builder.Configuration.AddJsonFile("config.json", optional: false, reloadOnChange: true);

// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register SentimentService with a typed HttpClient
builder.Services.AddHttpClient<ISentimentService, SentimentService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = config["AzureAI:Endpoint"]!.TrimEnd('/');
    client.BaseAddress = new Uri(endpoint + "/");
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config["AzureAI:ApiKey"]);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
