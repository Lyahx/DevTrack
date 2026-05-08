using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.QuickCapture;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class QuickCaptureService : IQuickCaptureService
{
    private readonly IIdeaService _ideas;

    public QuickCaptureService(IIdeaService ideas)
    {
        _ideas = ideas;
    }

    public Task<IdeaResponse> CaptureAsync(QuickCaptureRequest request, CancellationToken ct = default)
    {
        return _ideas.CreateAsync(new IdeaCreateRequest
        {
            Owner = request.Owner,
            Content = request.Content
        }, ct);
    }
}
