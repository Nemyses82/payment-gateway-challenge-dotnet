using System.Text.Json.Serialization;

namespace PaymentGateway.Processor.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Pending = 0,
    Authorized = 1,
    Declined = 2,
    Rejected = 3
}