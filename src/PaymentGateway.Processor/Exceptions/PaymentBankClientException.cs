namespace PaymentGateway.Processor.Exceptions;

public class PaymentBankClientException(string message) : Exception(message);