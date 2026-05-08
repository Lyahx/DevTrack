using DevTrack.Domain.DTOs.Decisions;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class DecisionCreateRequestValidator : AbstractValidator<DecisionCreateRequest>
{
    public DecisionCreateRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Reasoning).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Alternatives).MaximumLength(4000);
    }
}

public class DecisionUpdateRequestValidator : AbstractValidator<DecisionUpdateRequest>
{
    public DecisionUpdateRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Reasoning).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Alternatives).MaximumLength(4000);
    }
}
