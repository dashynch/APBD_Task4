namespace LegacyRenewalApp.Interfaces
{

    public interface IFeeCalculator
    { 
        decimal CalculateFee(decimal subtotalAfterDiscount, decimal supportFee, string normalizedPaymentMethod, out string notes);
    }
}