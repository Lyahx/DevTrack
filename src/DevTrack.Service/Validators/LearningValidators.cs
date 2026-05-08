using DevTrack.Domain.DTOs.LearningTracks;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class LearningTrackCreateRequestValidator : AbstractValidator<LearningTrackCreateRequest>
{
    public LearningTrackCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Source).MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class LearningTrackUpdateRequestValidator : AbstractValidator<LearningTrackUpdateRequest>
{
    public LearningTrackUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Source).MaximumLength(200);
    }
}

public class LearningTrackStatusUpdateRequestValidator : AbstractValidator<LearningTrackStatusUpdateRequest>
{
    public LearningTrackStatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class LearningModuleCreateRequestValidator : AbstractValidator<LearningModuleCreateRequest>
{
    public LearningModuleCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class LearningModuleUpdateRequestValidator : AbstractValidator<LearningModuleUpdateRequest>
{
    public LearningModuleUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public class LearningModuleStatusUpdateRequestValidator : AbstractValidator<LearningModuleStatusUpdateRequest>
{
    public LearningModuleStatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class LearningModuleOrderUpdateRequestValidator : AbstractValidator<LearningModuleOrderUpdateRequest>
{
    public LearningModuleOrderUpdateRequestValidator()
    {
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
