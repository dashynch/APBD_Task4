namespace LegacyRenewalApp.Interfaces
{

    public interface ISupportFeeCalculator
    {
        decimal CalculateFee(string normalizedPlanCode, bool includePremiumSupport, out string note);
    

    }
}