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
            // Reserve enough space for the JSON output. Without this, some LM Studio
            // builds default to ~16-256 tokens and silently truncate to empty content.
            max_tokens = 4096,
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

        var rawBody = await resp.Content.ReadAsStringAsync(ct);
        ChatCompletionResponse? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<ChatCompletionResponse>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            envelope = null;
        }

        if (envelope is null)
        {
            _logger.LogWarning("AI envelope unparsable. Body head: {Head}", rawBody[..Math.Min(rawBody.Length, 500)]);
            throw new AiServiceException("AI_BAD_RESPONSE", "AI sağlayıcı geçersiz yanıt verdi.");
        }

        var content = envelope.Choices?.FirstOrDefault()?.Message?.Content;
        var finishReason = envelope.Choices?.FirstOrDefault()?.FinishReason;
        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogWarning(
                "AI provider returned empty content. finish_reason={Reason} usage={Usage} body={Body}",
                finishReason ?? "(none)",
                envelope.Usage is null ? "(none)" : $"prompt={envelope.Usage.PromptTokens}, completion={envelope.Usage.CompletionTokens}, total={envelope.Usage.TotalTokens}",
                rawBody.Length > 800 ? rawBody[..800] + "…" : rawBody);
            var hint = finishReason switch
            {
                "length" => "Model max_tokens'a takıldı — daha büyük max_tokens veya context'i daha geniş bir model dene.",
                "content_filter" => "İçerik filtreye takıldı.",
                _ => "Transcript çok uzun olabilir veya model JSON üretemedi. Daha kısa transcript veya daha büyük model dene.",
            };
            throw new AiServiceException("AI_EMPTY_CONTENT", $"AI sağlayıcı boş içerik döndü. {hint}");
        }

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
        public Usage? Usage { get; set; }
    }

    private class ChatChoice
    {
        public ChatMessage? Message { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private class ChatMessage
    {
        public string? Content { get; set; }
    }

    private class Usage
    {
        [System.Text.Json.Serialization.JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
