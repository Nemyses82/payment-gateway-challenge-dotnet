using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IPaymentsProvider paymentsProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePaymentAsync(PostPaymentRequest request)
    {
        var paymentResponse = await paymentsProvider.CreatePaymentAsync(request);
        return Ok(paymentResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentAsync(Guid id)
    {
        try
        {
            var paymentResponse = await paymentsProvider.GetPayment(id);
            return Ok(paymentResponse);
        }
        catch (PaymentNotFoundException)
        {
            return NotFound();
        }
    }
}