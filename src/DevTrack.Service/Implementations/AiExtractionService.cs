using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevTrack.Domain.DTOs.AiImport;
using DevTrack.Domain.Exceptions;
using DevTrack.Service.Configuration;
using DevTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevTrack.Service.Implementations;

public class AiExtractionService : IAiExtractionService
{
    public const string HttpClientName = "Ai";

    private readonly IHttpClientFactory _httpFactory;
    private readonly AiSettings _settings;
    private readonly ILogger<AiExtractionService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public AiExtractionService(
        IHttpClientFactory httpFactory,
        IOptions<AiSettings> settings,
        ILogger<AiExtractionService> logger)
    {
        _httpFactory = httpFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<AiExtractionResult> ExtractAsync(string transcript, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(transcript))
            throw new AiServiceException("AI_TRANSCRIPT_EMPTY", "Transcript boş olamaz.", 400);

        var http = _httpFactory.CreateClient(HttpClientName);
        http.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        if (!string.IsNullOrEmpty(_settings.ApiKey))
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        var systemPrompt = BuildSystemPrompt();
        var userPrompt = $"Transcript:\n<<<\n{transcript}\n>>>";

        // response_format intentionally omitted — Gemma-style models on LM Studio reject
        // {"type":"json_object"} (only json_schema/text accepted). Prompt enforces JSON-only,
        // and StripCodeFence handles models that wrap output in ```json fences.
        var requestBody = new
        {
            model = _settings.Model,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt },
            },
            temperature = _settings.Temperature,
        };

        var url = $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";
        HttpResponseMessage resp;
        try
        {
            resp = await http.PostAsJsonAsync(url, requestBody, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AI provider unreachable at {Url}", url);
            throw new AiServiceException("AI_UNREACHABLE", $"AI sağlayıcıya ({_settings.BaseUrl}) bağlanılamadı. LM Studio / Ollama çalışıyor mu?", 503);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "AI request timed out");
            throw new AiServiceException("AI_TIMEOUT", $"AI yanıtı {_settings.TimeoutSeconds}s içinde gelmedi.", 504);
        }

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("AI provider returned {Status}: {Body}", resp.StatusCode, body);
            throw new AiServiceException("AI_PROVIDER_ERROR", $"AI sağlayıcı hata döndü: {resp.StatusCode}");
        }

        var envelope = await resp.Content.ReadFromJsonAsync<ChatCompletionResponse>(JsonOptions, ct)
            ?? throw new AiServiceException("AI_BAD_RESPONSE", "AI sağlayıcı geçersiz yanıt verdi.");

        var content = envelope.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(content))
            throw new AiServiceException("AI_EMPTY_CONTENT", "AI sağlayıcı boş içerik döndü.");

        // Some providers wrap JSON in ```json ... ``` fences; strip if present.
        content = StripCodeFence(content);

        try
        {
            var result = JsonSerializer.Deserialize<AiExtractionResult>(content, JsonOptions)
                ?? throw new AiServiceException("AI_PARSE_FAIL", "AI yanıtı çözümlenemedi.");
            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI JSON. Content head: {Head}", content[..Math.Min(content.Length, 500)]);
            throw new AiServiceException("AI_PARSE_FAIL", "AI yanıtı geçerli JSON değil.");
        }
    }

    private static string StripCodeFence(string content)
    {
        var trimmed = content.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline > 0) trimmed = trimmed[(firstNewline + 1)..];
            if (trimmed.EndsWith("```")) trimmed = trimmed[..^3];
        }
        return trimmed.Trim();
    }

    private static string BuildSystemPrompt() => """
You extract structured items from a learning conversation transcript (often between a user and an AI assistant). Return ONLY valid JSON, no commentary, no code fences.

Output schema:
{
  "worklogs": [{"whatIDid": "string", "whatsLeft": "string or null"}],
  "decisions": [{"title": "string", "reasoning": "string", "alternatives": "string or null"}],
  "nextSteps": [{"description": "string", "priority": "Low" | "Medium" | "High"}],
  "ideas": [{"content": "string"}],
  "resources": [{"title": "string", "url": "string", "type": "Documentation" | "Article" | "Video" | "GitHub" | "StackOverflow" | "ClaudeChat" | "Other", "notes": "string or null"}]
}

Rules:
- Preserve the original language of the transcript (Turkish stays Turkish, English stays English).
- worklogs = concrete things the user said they did, learned, or worked through. Brief sentences.
- decisions = explicit choices the user made with reasoning. "I chose X because Y" pattern.
- nextSteps = action items, follow-ups, "I should X next", "yarın bunu yapmalıyım". Assign priority by urgency cues; default Medium.
- ideas = thoughts to explore later, "şunu da deneyebilirim", possibilities not committed to.
- resources = external URLs mentioned (docs, articles, videos, repos). Pick the best-fit type.
- If a category has no items, return an empty array [].
- Do not invent items not grounded in the transcript.
- Keep each item concise. No nested explanations beyond the listed fields.

Output ONLY the JSON object.
""";

    private class ChatCompletionResponse
    {
        public List<ChatChoice>? Choices { get; set; }
    }

    private class ChatChoice
    {
        public ChatMessage? Message { get; set; }
    }

    private class ChatMessage
    {
        public string? Content { get; set; }
    }
}
