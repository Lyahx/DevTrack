using DevTrack.Domain.DTOs.Projects;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class ProjectCreateRequestValidator : AbstractValidator<ProjectCreateRequest>
{
    public ProjectCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Goal).MaximumLength(1000);
        RuleFor(x => x.RepoUrl).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class ProjectUpdateRequestValidator : AbstractValidator<ProjectUpdateRequest>
{
    public ProjectUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Goal).MaximumLength(1000);
        RuleFor(x => x.RepoUrl).MaximumLength(500);
    }
}

public class ProjectStatusUpdateRequestValidator : AbstractValidator<ProjectStatusUpdateRequest>
{
    public ProjectStatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
