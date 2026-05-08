using DevTrack.Domain.DTOs.Ideas;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class IdeaCreateRequestValidator : AbstractValidator<IdeaCreateRequest>
{
    public IdeaCreateRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}

public class IdeaUpdateRequestValidator : AbstractValidator<IdeaUpdateRequest>
{
    public IdeaUpdateRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}

public class IdeaConvertRequestValidator : AbstractValidator<IdeaConvertRequest>
{
    public IdeaConvertRequestValidator()
    {
        RuleFor(x => x.Priority).IsInEnum();
    }
}
