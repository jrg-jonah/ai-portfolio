using AiPortfolio.Api.Domain.Models;

namespace AiPortfolio.Api.Application.Interfaces;

public interface ISentimentService
{
    Task<SentimentResult> AnalyzeAsync(string text);
}
