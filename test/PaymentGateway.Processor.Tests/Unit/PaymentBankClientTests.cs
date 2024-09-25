using System.Net;

using FluentAssertions;

using PaymentGateway.Processor.Configuration;
using PaymentGateway.Processor.Enums;
using PaymentGateway.Processor.Exceptions;
using PaymentGateway.Processor.Models;
using PaymentGateway.Processor.Services;

using RichardSzalay.MockHttp;

namespace PaymentGateway.Processor.Tests.Unit;

[TestFixture]
public class PaymentBankClientTests
{
    private PaymentBankClient _sut;
    private MockHttpMessageHandler _mockHttp;
    private Payment _payment;
    const string PaymentIssuerBankBaseUrl = "http://localhost/simulator";

    [TearDown]
    public void TearDown() => _mockHttp.Dispose();

    [SetUp]
    public void SetUp()
    {
        _payment = new Payment(Guid.NewGuid(), PaymentStatus.Authorized, 42, "GBP",
            new PaymentDetails(Guid.NewGuid(), new CardDetails("4242424242424242", 1, 99, "123")));

        _mockHttp = new MockHttpMessageHandler();

        var client = _mockHttp.ToHttpClient();

        _sut = new PaymentBankClient(client, new ServiceConfig { PaymentIssuerBankBaseUrl = PaymentIssuerBankBaseUrl });
    }

    [Test]
    public async Task Should_Return_A_Valid_Bank_Payment_Response()
    {
        _mockHttp
            .When(PaymentIssuerBankBaseUrl)
            .Respond("application/json",
                "{\"authorized\" : true, \"authorization_code\" : \"0bb07405-6d44-4b50-a14f-7ae0beff13ad\"}");

        var bankPaymentResponse = await _sut.IssuePaymentAsync(_payment);

        bankPaymentResponse.Should().NotBeNull();
        bankPaymentResponse.IsPaymentAuthorized.Should().BeTrue();
        bankPaymentResponse.AuthorizationCode.Should().Be("0bb07405-6d44-4b50-a14f-7ae0beff13ad");
    }

    [Test]
    public void Should_Throw_A_PaymentBankClientException_When_Response_Is_Not_Valid()
    {
        _mockHttp
            .When(PaymentIssuerBankBaseUrl)
            .Respond("application/json", string.Empty);

        var exception = Assert.ThrowsAsync<PaymentBankClientException>(() => _sut.IssuePaymentAsync(_payment));

        exception.Should().NotBeNull();
        exception.Message.Should().Be("Unexpected error while contacting Bank Client Simulator");
    }

    [Test]
    public void Should_Throw_A_PaymentBankClientException_When_Http_Call_Is_Not_Successful()
    {
        _mockHttp
            .When(PaymentIssuerBankBaseUrl)
            .Respond(HttpStatusCode.BadRequest);

        var exception = Assert.ThrowsAsync<PaymentBankClientException>(() => _sut.IssuePaymentAsync(_payment));

        exception.Should().NotBeNull();
        exception.Message.Should().Be("Unexpected error while contacting Bank Client Simulator");
    }
}