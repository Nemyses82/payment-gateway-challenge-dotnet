namespace PaymentGateway.Processor.Models;

public sealed record PaymentDetails(Guid Id, CardDetails CardDetails);

public sealed record CardDetails(string CardNumber, int ExpiryMonth, int ExpiryYear, string CVV)
{
    public int GetCardNumberLastFour() => Convert.ToInt16(CardNumber[^4..]);
}