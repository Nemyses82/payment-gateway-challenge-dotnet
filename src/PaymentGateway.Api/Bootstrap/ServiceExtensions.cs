using PaymentGateway.Api.Services;
using PaymentGateway.Processor.Services;

namespace PaymentGateway.Api.Bootstrap;

public static class ServiceExtensions
{
    public static void AddPaymentServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, MemoryPaymentsRepository>();
        services.AddSingleton<IPaymentsProvider, PaymentsProvider>();
        services.AddSingleton<IPaymentBankClient, PaymentBankClient>().AddHttpClient();
    }
}