using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper
{

    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        public decimal CalculateFee(string normalizedPlanCode, bool includePremiumSupport, out string notes)
        {
            decimal supportFee = 0m;
            notes = string.Empty;
            
            if (includePremiumSupport)
            {
                if (normalizedPlanCode == "START")
                {
                    supportFee = 250m;
                }
                else if (normalizedPlanCode == "PRO")
                {
                    supportFee = 400m;
                }
                else if (normalizedPlanCode == "ENTERPRISE")
                {
                    supportFee = 700m;
                }

                notes += "premium support included; ";
            }

            return supportFee;
        }
    }
}