using FluentAssertions;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Unit;

/// <summary>
/// Note for the reviewer: I am aware that I didn't cover every single case scenario or failure.
/// The reason is that I preferred investing more time in other components implementation which I thought it could offer more value at the final evaluation objective
/// </summary>
[TestFixture]
public class PostPaymentRequestValidatorTests
{
    private PostPaymentRequestValidator _sut;
    private PostPaymentRequest _postPaymentRequest;


    [SetUp]
    public void SetUp()
    {
        _postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = DateTime.Today.Month,
            ExpiryYear = DateTime.Today.Year,
            Currency = "GBP",
            Amount = 1,
            Cvv = "123"
        };
        _sut = new PostPaymentRequestValidator();
    }

    [Test]
    public void Should_Not_Return_When_Request_Is_Valid()
    {
        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Should_Return_Error_When_CardNumber_Is_Not_Valid(string? cardNumber)
    {
        _postPaymentRequest.CardNumber = cardNumber;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .Contain(failure => failure.ErrorMessage == "CardNumber cannot be empty or null.");
    }

    [Test]
    public void Should_Return_Error_When_ExpiryMonth_Is_Not_Valid()
    {
        _postPaymentRequest.ExpiryMonth = 13;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .Contain(failure => failure.ErrorMessage == "ExpiryMonth must be between 1 and 12.");
    }

    [Test]
    public void Should_Return_Error_When_ExpiryYear_Is_Not_Valid()
    {
        _postPaymentRequest.ExpiryYear = 1900;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(failure =>
            failure.ErrorMessage == "ExpiryYear must be greater or equal to current year date.");
    }

    [Test]
    public void Should_Return_Error_When_Card_Is_Expired()
    {
        _postPaymentRequest.ExpiryMonth = 1;
        _postPaymentRequest.ExpiryYear = 1900;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(failure =>
            failure.ErrorMessage == "Expiry Month and Expiry Year must be in the future.");
    }

    [Test]
    public void Should_Return_Error_When_Currency_Has_Value_Not_Allowed()
    {
        _postPaymentRequest.Currency = "JPY";

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .Contain(failure => failure.ErrorMessage == "Currency must contain only GBP, EUR or USD.");
    }

    [Test]
    public void Should_Return_Error_When_Amount_Is_Not_Valid()
    {
        _postPaymentRequest.Amount = 0;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(failure => failure.ErrorMessage == "Amount cannot be empty or null.");
    }

    [TestCase(null)]
    public void Should_Return_Error_When_CVV_Is_Not_Valid(string? cvv)
    {
        _postPaymentRequest.Cvv = cvv;

        var validationResult = _sut.Validate(_postPaymentRequest);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(failure => failure.ErrorMessage == "CVV cannot be empty or null.");
    }
}