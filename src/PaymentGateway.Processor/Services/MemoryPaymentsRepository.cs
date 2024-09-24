using System.Collections.Concurrent;
using PaymentGateway.Processor.Models;

namespace PaymentGateway.Processor.Services;

public interface IPaymentsRepository
{
    void Add(Payment payment);
    Payment? Get(Guid id);
}

public class MemoryPaymentsRepository : IPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _paymentRepository = new();

    public void Add(Payment payment) => _paymentRepository.TryAdd(payment.Id, payment);

    public Payment? Get(Guid id) => _paymentRepository.GetValueOrDefault(id);
}