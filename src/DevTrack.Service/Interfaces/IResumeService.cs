using DevTrack.Domain.DTOs.Resume;

namespace DevTrack.Service.Interfaces;

public interface IResumeService
{
    Task<ResumeProjectResponse> GetForProjectAsync(int projectId, CancellationToken ct = default);
    Task<ResumeComponentResponse> GetForComponentAsync(int componentId, CancellationToken ct = default);
    Task<ResumeLearningTrackResponse> GetForLearningTrackAsync(int trackId, CancellationToken ct = default);
    Task<ResumeLearningModuleResponse> GetForLearningModuleAsync(int moduleId, CancellationToken ct = default);
}
