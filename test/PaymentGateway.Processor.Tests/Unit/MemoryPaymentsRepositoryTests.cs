using FluentAssertions;

using PaymentGateway.Processor.Enums;
using PaymentGateway.Processor.Models;
using PaymentGateway.Processor.Services;

namespace PaymentGateway.Processor.Tests.Unit;

[TestFixture]
public class MemoryPaymentsRepositoryTests
{
    private MemoryPaymentsRepository _sut;
    private Payment _payment;

    [SetUp]
    public void SetUp()
    {
        _payment = new Payment(Guid.NewGuid(), PaymentStatus.Authorized, 42, "GBP",
            new PaymentDetails(Guid.NewGuid(), new CardDetails("4242424242424242", 1, 99, "123")));

        _sut = new MemoryPaymentsRepository();
    }

    [Test]
    public void Should_Return_Payment_When_Payment_Is_Stored_In_Memory()
    {
        _sut.Add(_payment);

        var paymentRetrieved = _sut.Get(_payment.Id);

        paymentRetrieved.Should().BeEquivalentTo(_payment);
    }

    [Test]
    public void Should_Return_Null_When_Payment_Is_Not_Existing()
    {
        _sut.Add(_payment);

        var paymentRetrieved = _sut.Get(Guid.NewGuid());

        paymentRetrieved.Should().BeNull();
    }
}