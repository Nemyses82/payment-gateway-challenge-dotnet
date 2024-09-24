using PaymentGateway.Processor.Enums;

namespace PaymentGateway.Processor.Models;

public sealed record Payment(Guid Id, PaymentStatus Status, int Amount, string Currency, PaymentDetails PaymentDetails);