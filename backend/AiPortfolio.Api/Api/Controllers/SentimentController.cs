using AiPortfolio.Api.Application.Interfaces;
using AiPortfolio.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AiPortfolio.Api.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SentimentController : ControllerBase
{
    private readonly ISentimentService _sentimentService;

    public SentimentController(ISentimentService sentimentService)
    {
        _sentimentService = sentimentService;
    }

    [HttpPost]
    public async Task<ActionResult<SentimentResult>> Analyze([FromBody] SentimentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var result = await _sentimentService.AnalyzeAsync(request.Text);
        return Ok(result);
    }
}
