using DevTrack.Domain.DTOs.Tags;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DevTrack.Service.Validators;

public class TagCreateRequestValidator : AbstractValidator<TagCreateRequest>
{
    public TagCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color)
            .Must(BeValidHexColor)
            .When(x => !string.IsNullOrWhiteSpace(x.Color))
            .WithMessage("Color must be a hex code like #RRGGBB.");
    }

    private static bool BeValidHexColor(string? color) =>
        color is not null && Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$");
}

public class TagUpdateRequestValidator : AbstractValidator<TagUpdateRequest>
{
    public TagUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color)
            .Must(BeValidHexColor)
            .When(x => !string.IsNullOrWhiteSpace(x.Color))
            .WithMessage("Color must be a hex code like #RRGGBB.");
    }

    private static bool BeValidHexColor(string? color) =>
        color is not null && Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$");
}
