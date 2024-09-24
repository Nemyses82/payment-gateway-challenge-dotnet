using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Processor.Services;

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
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };

        // Act
        var response = await _client.PostAsync(UriPath, CreatePaymentRequestContent(payment));

        // Arrange
        var repository = _factory.Services.GetRequiredService<IPaymentsRepository>();
    }

    // [Test]
    // public async Task Should_Return_A_Payment_When_A_Valid_Request_Is_Provided()
    // {
    //     // Arrange
    //     var payment = new PostPaymentResponse
    //     {
    //         Id = Guid.NewGuid(),
    //         ExpiryYear = _random.Next(2023, 2030),
    //         ExpiryMonth = _random.Next(1, 12),
    //         Amount = _random.Next(1, 10000),
    //         CardNumberLastFour = _random.Next(1111, 9999),
    //         Currency = "GBP"
    //     };
    //     var repository = _factory.Services.GetRequiredService<IPaymentsRepository>();
    //     repository.Add(payment);
    //
    //     // Act
    //     var response = await _client.GetAsync($"{UriPath}{payment.Id}");
    //     var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
    //
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     Assert.That(paymentResponse, Is.Not.Null);
    // }

    [Test]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{UriPath}{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private static StringContent CreatePaymentRequestContent(PostPaymentResponse paymentResponse) =>
        new(JsonSerializer.Serialize(paymentResponse), Encoding.UTF8, "application/json");

    [TearDown]
    public void TearDown() => _client.Dispose();

    [OneTimeTearDown]
    public void OneTimeTearDown() => _factory.Dispose();
}