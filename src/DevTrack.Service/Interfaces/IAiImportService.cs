using DevTrack.Domain.DTOs.AiImport;

namespace DevTrack.Service.Interfaces;

public interface IAiImportService
{
    Task<AiImportApplyResult> ApplyToLearningTrackAsync(int trackId, AiImportApplyRequest request, CancellationToken ct = default);
}
