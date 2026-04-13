using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper
{

    public class FeeCalculator : IFeeCalculator
    {
        public decimal CalculateFee(decimal subtotalAfterDiscount, decimal supportFee, string normalizedPaymentMethod, out string notes)
    
        {
            decimal paymentFee = 0m;
            notes = string.Empty;
            if (normalizedPaymentMethod == "CARD")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.02m;
                notes += "card payment fee; ";
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.01m;
                notes += "bank transfer fee; ";
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.035m;
                notes += "paypal fee; ";
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                paymentFee = 0m;
                notes += "invoice payment; ";
            }
            else
            {
                throw new ArgumentException("Unsupported payment method");
            }

            return paymentFee;
        }
    }
}