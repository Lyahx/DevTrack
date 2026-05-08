using DevTrack.Domain.Common;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class OwnerReferenceValidator : AbstractValidator<OwnerReference>
{
    public OwnerReferenceValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
