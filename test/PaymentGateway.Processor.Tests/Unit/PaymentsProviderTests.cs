using AutoFixture;

using FluentAssertions;

using Moq;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Processor.Models;
using PaymentGateway.Processor.Services;

namespace PaymentGateway.Processor.Tests.Unit;

[TestFixture]
public class PaymentsProviderTests
{
    private Mock<IPaymentBankClient> _paymentBankClient;
    private Mock<IPaymentsRepository> _paymentsRepository;
    
    private PaymentsProvider _sut;
    
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        
        _paymentBankClient = new Mock<IPaymentBankClient>();
        _paymentsRepository = new Mock<IPaymentsRepository>();

        _sut = new PaymentsProvider(_paymentBankClient.Object, _paymentsRepository.Object);
    }

    [Test]
    public async Task Should_Return_Payment_From_Repository_If_Payment_Is_Found()
    {
        var paymentId = Guid.NewGuid();
        var cardDetails = _fixture.Build<CardDetails>()
            .With(x => x.CardNumber, "12345678").Create();        
        var paymentDetails = _fixture.Build<PaymentDetails>()
            .With(x => x.CardDetails, cardDetails).Create();        
        var payment = _fixture.Build<Payment>()
            .With(x => x.Id, paymentId)
            .With(x => x.PaymentDetails, paymentDetails)
            .Create();
        _paymentsRepository.Setup(x => x.Get(paymentId)).Returns(payment);

        var paymentResponse = await _sut.GetPayment(paymentId);
        
        paymentResponse.Should().NotBeNull();
        paymentResponse.Id.Should().Be(paymentId);
    }
}