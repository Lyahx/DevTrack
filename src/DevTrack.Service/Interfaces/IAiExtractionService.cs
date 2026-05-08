using DevTrack.Domain.DTOs.AiImport;

namespace DevTrack.Service.Interfaces;

public interface IAiExtractionService
{
    Task<AiExtractionResult> ExtractAsync(string transcript, CancellationToken ct = default);
}
