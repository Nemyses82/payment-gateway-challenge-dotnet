﻿using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Processor.Enums;
using PaymentGateway.Processor.Services;

namespace PaymentGateway.Api.Services;

public interface IPaymentsProvider
{
    Task<PostPaymentResponse> CreatePaymentAsync(PostPaymentRequest request);
    Task<GetPaymentResponse> GetPayment(Guid id);
}

/// <summary>
/// This provider wants to be an abstraction layer to do business logic, without polluting the controller
/// </summary>
public class PaymentsProvider(
    IPaymentBankClient paymentBankClient,
    IPaymentsRepository repository,
    ILogger<PaymentsProvider> logger) : IPaymentsProvider
{
    public Task<GetPaymentResponse> GetPayment(Guid id)
    {
        try
        {
            var payment = repository.Get(id) ?? throw new PaymentNotFoundException($"Payment with id {id} not found");
            return Task.FromResult(payment.ToGetPaymentResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"An error occured while getting payment: {e.Message}");
            throw;
        }
    }

    public async Task<PostPaymentResponse> CreatePaymentAsync(PostPaymentRequest request)
    {
        try
        {
            var payment = request.ToPayment();
            var bankResponse = await paymentBankClient.IssuePaymentAsync(payment);

            // Here PaymentStatus result is simplified just to Authorized or Declined
            payment = payment with
            {
                Status = bankResponse.IsPaymentAuthorized ? PaymentStatus.Authorized : PaymentStatus.Declined
            };
            repository.Add(payment);

            return payment.ToPostPaymentResponse();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while creating payment");
            throw;
        }
    }
}