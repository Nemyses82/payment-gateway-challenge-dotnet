using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Processor.Enums;

namespace PaymentGateway.Api.Tests.Integration.Controllers;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((host, configurationBuilder) => { });
    }
}

[TestFixture]
public class PaymentsControllerTests
{
    private IntegrationTestWebApplicationFactory _factory;
    private HttpClient _client;

    private Random _random;

    private const string UriPath = "/api/payments/";

    [OneTimeSetUp]
    public void OneTimeSetup() => _factory = new IntegrationTestWebApplicationFactory();

    [SetUp]
    public void Setup()
    {
        _random = new Random();
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task Should_Create_Payment()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryYear = 2025,
            ExpiryMonth = 4,
            Amount = 1,
            Currency = "GBP",
            Cvv = "123"
        };

        // Act
        var response = await _client.PostAsync(UriPath, CreatePaymentRequestContent(payment));

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        paymentResponse.Should().NotBeNull();
        response.Headers.Location!.AbsolutePath.Should().Be($"/api/payments/{paymentResponse.Id}");
    }

    [Test]
    public async Task Should_Return_A_Payment_Details_When_A_Payment_Is_Existing_And_Authorized()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryYear = 2025,
            ExpiryMonth = 4,
            Amount = 1,
            Currency = "GBP",
            Cvv = "123"
        };
        var paymentResult = await _client.PostAsync(UriPath, CreatePaymentRequestContent(payment));
        var paymentResponse = await paymentResult.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Act
        var paymentDetailsResult = await _client.GetAsync($"{UriPath}{paymentResponse.Id}");
        var paymentDetailsResponse = await paymentDetailsResult.Content.ReadFromJsonAsync<GetPaymentResponse>();

        // Assert
        paymentDetailsResult.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentDetailsResponse.Should().NotBeNull();
        paymentDetailsResponse.Id.Should().Be(paymentResponse.Id);
        paymentDetailsResponse.CardNumberLastFour.Should().Be(8877);
        paymentDetailsResponse.ExpiryYear.Should().Be(2025);
        paymentDetailsResponse.ExpiryMonth.Should().Be(4);
        paymentDetailsResponse.Amount.Should().Be(1);
        paymentDetailsResponse.Currency.Should().Be("GBP");
        paymentDetailsResponse.Status.Should().Be(PaymentStatus.Authorized);
    }

    [Test]
    public async Task Should_Return_A_Payment_Details_When_A_Payment_Is_Existing_And_Declined()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryYear = 2026,
            ExpiryMonth = 1,
            Amount = 600,
            Currency = "USD",
            Cvv = "456"
        };
        var paymentResult = await _client.PostAsync(UriPath, CreatePaymentRequestContent(payment));
        var paymentResponse = await paymentResult.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Act
        var paymentDetailsResult = await _client.GetAsync($"{UriPath}{paymentResponse.Id}");
        var paymentDetailsResponse = await paymentDetailsResult.Content.ReadFromJsonAsync<GetPaymentResponse>();

        // Assert
        paymentDetailsResult.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentDetailsResponse.Should().NotBeNull();
        paymentDetailsResponse.Id.Should().Be(paymentResponse.Id);
        paymentDetailsResponse.CardNumberLastFour.Should().Be(8112);
        paymentDetailsResponse.ExpiryYear.Should().Be(2026);
        paymentDetailsResponse.ExpiryMonth.Should().Be(1);
        paymentDetailsResponse.Amount.Should().Be(600);
        paymentDetailsResponse.Currency.Should().Be("USD");
        paymentDetailsResponse.Status.Should().Be(PaymentStatus.Declined);
    }

    [Test]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{UriPath}{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private static StringContent CreatePaymentRequestContent(PostPaymentRequest paymentRequest) =>
        new(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");

    [TearDown]
    public void TearDown() => _client.Dispose();

    [OneTimeTearDown]
    public void OneTimeTearDown() => _factory.Dispose();
}