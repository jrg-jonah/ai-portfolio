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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

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
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");
app.MapControllers();

app.Run();
