using Microsoft.Extensions.Http.Resilience;

using PaymentGateway.Api.Services;
using PaymentGateway.Processor.Services;

using Polly;
using Polly.Retry;

namespace PaymentGateway.Api.Bootstrap;

public static class ServiceExtensions
{
    public static void AddPaymentServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, MemoryPaymentsRepository>();
        services.AddSingleton<IPaymentsProvider, PaymentsProvider>();

        services.AddHttpClient<IPaymentBankClient, PaymentBankClient>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry = new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = OnRetry
                };
            });
    }

    private static ValueTask OnRetry(OnRetryArguments<HttpResponseMessage> arg)
    {
        Console.WriteLine($"RetryDelay: {arg.RetryDelay} - AttemptNumber: {arg.AttemptNumber}");
        return ValueTask.CompletedTask;
    }
}