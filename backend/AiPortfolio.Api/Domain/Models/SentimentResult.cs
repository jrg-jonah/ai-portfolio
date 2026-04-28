namespace AiPortfolio.Api.Domain.Models;

public class SentimentRequest
{
    public string Text { get; set; } = string.Empty;
}

public class SentimentResult
{
    public string Sentiment { get; set; } = string.Empty;
    public SentimentScores Scores { get; set; } = new();
}

public class SentimentScores
{
    public double Positive { get; set; }
    public double Neutral { get; set; }
    public double Negative { get; set; }
}
