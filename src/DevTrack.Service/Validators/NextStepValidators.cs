using DevTrack.Domain.DTOs.NextSteps;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class NextStepCreateRequestValidator : AbstractValidator<NextStepCreateRequest>
{
    public NextStepCreateRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class NextStepUpdateRequestValidator : AbstractValidator<NextStepUpdateRequest>
{
    public NextStepUpdateRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}
