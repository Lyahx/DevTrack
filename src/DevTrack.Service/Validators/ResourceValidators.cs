using DevTrack.Domain.DTOs.Resources;
using FluentValidation;

namespace DevTrack.Service.Validators;

public class ResourceCreateRequestValidator : AbstractValidator<ResourceCreateRequest>
{
    public ResourceCreateRequestValidator()
    {
        RuleFor(x => x.Owner).NotNull().SetValidator(new OwnerReferenceValidator());
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(1000).Must(BeAValidUrl).WithMessage("Url must be a valid http(s) URL.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }

    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

public class ResourceUpdateRequestValidator : AbstractValidator<ResourceUpdateRequest>
{
    public ResourceUpdateRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(1000).Must(BeAValidUrl).WithMessage("Url must be a valid http(s) URL.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }

    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
