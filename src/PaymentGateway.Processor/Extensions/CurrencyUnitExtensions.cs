namespace PaymentGateway.Processor.Extensions;

public static class CurrencyUnitExtensions
{
    public static int ToMinorCurrencyUnits(this int amount) => 100 * amount;
}