using DevTrack.Domain.DTOs.QuickCapture;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class QuickCaptureRequestValidator : AbstractValidator<QuickCaptureRequest>
{
    public QuickCaptureRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
