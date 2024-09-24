using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("CardNumber cannot be empty or null.")
            .Length(14, 19).WithMessage("CardNumber must be between 14 and 19 characters.")
            .Matches("^[0-9]+$").WithMessage("CardNumber must contain only numeric characters.");

        RuleFor(x => x.ExpiryMonth)
            .NotEmpty().WithMessage("ExpiryMonth cannot be empty or null.")
            .InclusiveBetween(1, 12).WithMessage("ExpiryMonth must be between 1 and 12.");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty().WithMessage("ExpiryYear cannot be empty or null.")
            .Must(x => x >= DateTime.Now.Year).WithMessage("ExpiryYear must be greater or equal to DateTime.Now year.");

        RuleFor(x => x).Must(NotBeExpiredCard)
            .OverridePropertyName("PostPaymentRequest:CardExpired")
            .WithMessage("Expiry Month and Expiry Year must be in the future.")
            .When(CanCreateAValidDate);

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency cannot be empty or null.")
            .Must(x => new List<string> { "GBP", "EUR", "CHF" }.Contains(x))
            .WithMessage("Currency must contain only GBP, EUR or CHF.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount cannot be empty or null.");

        RuleFor(x => x.Cvv)
            .NotEmpty().WithMessage("CVV cannot be empty or null.")
            .Length(3, 4).WithMessage("CVV must be between 3 and 4 characters.")
            .Matches("^[0-9]+$").WithMessage("CVV must contain only numeric characters.");
    }

    private static bool CanCreateAValidDate(PostPaymentRequest request)
    {
        return request.ExpiryYear >= 0 && request.ExpiryYear <= DateTime.Now.Year && request.ExpiryMonth >= 1 &&
               request.ExpiryMonth <= 12;
    }

    private static bool NotBeExpiredCard(PostPaymentRequest request)
    {
        var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        return new DateTime(request.ExpiryYear, request.ExpiryMonth, 1) >= currentDate;
    }
}