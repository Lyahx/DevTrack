using DevTrack.Domain.DTOs.Components;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class ComponentCreateRequestValidator : AbstractValidator<ComponentCreateRequest>
{
    public ComponentCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.TechStack).MaximumLength(500);
        RuleFor(x => x.LocalUrl).MaximumLength(500);
        RuleFor(x => x.RepoPath).MaximumLength(500);
        RuleFor(x => x.CurrentStatusNote).MaximumLength(2000);
    }
}

public class ComponentUpdateRequestValidator : AbstractValidator<ComponentUpdateRequest>
{
    public ComponentUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.TechStack).MaximumLength(500);
        RuleFor(x => x.LocalUrl).MaximumLength(500);
        RuleFor(x => x.RepoPath).MaximumLength(500);
        RuleFor(x => x.CurrentStatusNote).MaximumLength(2000);
    }
}

public class ComponentStatusNoteRequestValidator : AbstractValidator<ComponentStatusNoteRequest>
{
    public ComponentStatusNoteRequestValidator()
    {
        RuleFor(x => x.CurrentStatusNote).MaximumLength(2000);
    }
}
