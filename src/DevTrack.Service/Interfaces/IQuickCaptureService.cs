using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.QuickCapture;

namespace DevTrack.Service.Interfaces;

public interface IQuickCaptureService
{
    Task<IdeaResponse> CaptureAsync(QuickCaptureRequest request, CancellationToken ct = default);
}
