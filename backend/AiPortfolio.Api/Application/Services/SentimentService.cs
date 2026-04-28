using System.Net.Http.Headers;
using System.Text;
using AiPortfolio.Api.Application.Interfaces;
using AiPortfolio.Api.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AiPortfolio.Api.Application.Services;

public class SentimentService : ISentimentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SentimentService> _logger;

    public SentimentService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<SentimentService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SentimentResult> AnalyzeAsync(string text)
    {
        // Build the request body for Azure AI Language sentiment analysis
        var requestBody = new
        {
            kind = "SentimentAnalysis",
            parameters = new { modelVersion = "latest" },
            analysisInput = new
            {
                documents = new[]
                {
                    new { id = "1", language = "en", text }
                }
            }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending sentiment analysis request to Azure AI Language");

        var response = await _httpClient.PostAsync(
            "language/:analyze-text?api-version=2024-11-15-preview", content);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JObject.Parse(responseJson);

        // Navigate the Azure AI Language response structure
        var document = result["results"]?["documents"]?[0];
        var sentiment = document?["sentiment"]?.ToString() ?? "unknown";
        var confidence = document?["confidenceScores"];

        _logger.LogInformation("Sentiment result: {Sentiment}", sentiment);

        return new SentimentResult
        {
            Sentiment = sentiment,
            Scores = new SentimentScores
            {
                Positive = confidence?["positive"]?.Value<double>() ?? 0,
                Neutral = confidence?["neutral"]?.Value<double>() ?? 0,
                Negative = confidence?["negative"]?.Value<double>() ?? 0
            }
        };
    }
}
