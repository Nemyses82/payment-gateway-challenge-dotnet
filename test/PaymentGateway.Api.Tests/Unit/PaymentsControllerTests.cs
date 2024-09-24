using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Unit;

[TestFixture]
public class PaymentsControllerTests
{
    private Mock<IPaymentsProvider> _paymentsProvider;
    
    private PaymentsController _sut;
    
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _paymentsProvider = new Mock<IPaymentsProvider>();
        _sut = new PaymentsController(_paymentsProvider.Object);
    }

    [Test]
    public async Task Should_Return_A_Payment_If_Existing_Payment_Is_Found()
    {
        var paymentId = Guid.NewGuid();
        var paymentResponse = _fixture.Create<GetPaymentResponse>();
        _paymentsProvider.Setup(x => x.GetPayment(paymentId)).ReturnsAsync(paymentResponse).Verifiable();

        var actionResult = await _sut.GetPaymentAsync(paymentId);
        
        actionResult.Should().BeAssignableTo<OkObjectResult>();
        var objectResult = actionResult.As<ObjectResult>();
        
        objectResult.Should().NotBeNull();
        
        var resultValue = objectResult.Value;
        resultValue.Should().BeOfType<GetPaymentResponse>();   
    }
    
    [Test]
    public async Task Should_Return_NotFound_When_An_PaymentNotFoundException_Is_Raised()
    {
        var paymentId = Guid.NewGuid();
        _paymentsProvider.Setup(x => x.GetPayment(paymentId))
            .Throws(new PaymentNotFoundException($"Payment with id {paymentId} not found")).Verifiable();

        var actionResult = await _sut.GetPaymentAsync(paymentId);
        
        actionResult.Should().BeAssignableTo<NotFoundResult>();
        var objectResult = actionResult.As<ObjectResult>();
        
        objectResult.Should().BeNull();
    }    
}