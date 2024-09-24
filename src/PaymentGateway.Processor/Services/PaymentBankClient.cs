using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using PaymentGateway.Processor.Configuration;
using PaymentGateway.Processor.Exceptions;
using PaymentGateway.Processor.Extensions;
using PaymentGateway.Processor.Models;

namespace PaymentGateway.Processor.Services;

public interface IPaymentBankClient
{
    Task<BankPaymentResponse> IssuePaymentAsync(Payment payment);
}

public class PaymentBankClient(HttpClient httpClient, ServiceConfig serviceConfig) : IPaymentBankClient
{
    private static readonly JsonSerializerOptions Options = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<BankPaymentResponse> IssuePaymentAsync(Payment payment)
    {
        var requestBody = new BankPaymentRequest(
            payment.PaymentDetails.CardDetails.CardNumber,
            $"{payment.PaymentDetails.CardDetails.ExpiryMonth:00}/{payment.PaymentDetails.CardDetails.ExpiryYear}",
            payment.Currency,
            payment.Amount.ToMinorCurrencyUnits(),
            payment.PaymentDetails.CardDetails.CVV
        );

        var response = await httpClient.PostAsJsonAsync(serviceConfig.PaymentIssuerBankBaseUrl, requestBody, Options);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<BankPaymentResponse?>(responseBody) ??
               throw new PaymentBankClientException("Error while processing payment request");
    }
}

public sealed record BankPaymentRequest(
    [property: JsonPropertyName("card_number")]
    string CardNumber,
    [property: JsonPropertyName("expiry_date")]
    string ExpiryDate,
    string Currency,
    int Amount,
    string CVV);

public sealed record BankPaymentResponse(
    [property: JsonPropertyName("authorized")]
    bool IsPaymentAuthorized,
    [property: JsonPropertyName("authorization_code")]
    string AuthorizationCode);