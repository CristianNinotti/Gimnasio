// Application/Validation/UserRequestValidator.cs
using Application.Models.Request;
using FluentValidation;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleSet("Create", () =>
        {
            RuleFor(x => x.NameAccount).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        });

        RuleSet("Update", () =>
        {
            // Nada obligatorio; si viene, se valida formato
            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
                RuleFor(x => x.Email!).EmailAddress());
        });

        RuleSet("Login", () =>
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        });
    }
}
