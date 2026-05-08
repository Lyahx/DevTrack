using DevTrack.Domain.DTOs.Worklogs;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class WorklogCreateRequestValidator : AbstractValidator<WorklogCreateRequest>
{
    public WorklogCreateRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.WhatIDid).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.WhatsLeft).MaximumLength(4000);
    }
}

public class WorklogUpdateRequestValidator : AbstractValidator<WorklogUpdateRequest>
{
    public WorklogUpdateRequestValidator()
    {
        RuleFor(x => x.WhatIDid).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.WhatsLeft).MaximumLength(4000);
    }
}
