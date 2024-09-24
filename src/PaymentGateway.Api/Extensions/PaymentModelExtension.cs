using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Processor.Enums;
using PaymentGateway.Processor.Models;

namespace PaymentGateway.Api.Extensions;

public static class PaymentModelExtension
{
    public static GetPaymentResponse ToGetPaymentResponse(this Payment payment) => new()
    {
        Id = payment.Id,
        Amount = payment.Amount,
        ExpiryMonth = payment.PaymentDetails.CardDetails.ExpiryMonth,
        ExpiryYear = payment.PaymentDetails.CardDetails.ExpiryYear,
        CardNumberLastFour = payment.PaymentDetails.CardDetails.GetCardNumberLastFour(),
        Currency = payment.Currency,
        Status = payment.Status
    };

    public static PostPaymentResponse ToPostPaymentResponse(this Payment payment) => new()
    {
        Id = payment.Id,
        Status = payment.Status,
        CardNumberLastFour = payment.PaymentDetails.CardDetails.GetCardNumberLastFour(),
        ExpiryMonth = payment.PaymentDetails.CardDetails.ExpiryMonth,
        ExpiryYear = payment.PaymentDetails.CardDetails.ExpiryYear,
        Currency = payment.Currency,
        Amount = payment.Amount
    };

    public static Payment ToPayment(this PostPaymentRequest paymentRequest) =>
        new(Guid.NewGuid(),
            PaymentStatus.Pending,
            paymentRequest.Amount,
            paymentRequest.Currency,
            new PaymentDetails(Guid.NewGuid(),
                new CardDetails(paymentRequest.CardNumber, paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear,
                    paymentRequest.Cvv)));
}