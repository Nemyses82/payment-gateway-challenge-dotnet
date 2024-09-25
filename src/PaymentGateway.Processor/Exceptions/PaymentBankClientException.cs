namespace PaymentGateway.Processor.Exceptions;

public class PaymentBankClientException(string message, Exception exception) : Exception(message, exception);