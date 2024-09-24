using AutoFixture;
using FluentAssertions;
using Moq;
using PaymentGateway.Api.Models.Requests;
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

    [OneTimeSetUp]
    public void OneTimeSetUp() => _fixture = new Fixture();    
    
    [SetUp]
    public void SetUp()
    {
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
        paymentResponse.CardNumberLastFour.Should().Be(5678);
    }

    [Test]
    public async Task Should_Return_Payment_From_Repository_When_A_Payment_Is_Created()
    {
        var postPaymentRequest = _fixture.Build<PostPaymentRequest>()
            .With(x => x.CardNumber, "12345678").Create();
        var bankPaymentResponse = _fixture.Create<BankPaymentResponse>();
        _paymentBankClient.Setup(x => x.IssuePaymentAsync(It.IsAny<Payment>())).ReturnsAsync(bankPaymentResponse);

        var paymentResponse = await _sut.CreatePaymentAsync(postPaymentRequest);

        paymentResponse.Should().NotBeNull();
        paymentResponse.CardNumberLastFour.Should().Be(5678);
    }
}