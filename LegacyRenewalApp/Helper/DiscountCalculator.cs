using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper
{

    public class DiscountCalculator : IDiscountCalculator
    {
        public string Description =>  "Discount calculator";

        public decimal CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, out string notes)
        {
            decimal discountAmount = 0m;
            notes = string.Empty;
            if (customer.Segment == "Silver")
            {
                discountAmount += baseAmount * 0.05m;
                notes += "silver discount; ";
            }
            else if (customer.Segment == "Gold")
            {
                discountAmount += baseAmount * 0.10m;
                notes += "gold discount; ";
            }
            else if (customer.Segment == "Platinum")
            {
                discountAmount += baseAmount * 0.15m;
                notes += "platinum discount; ";
            }
            else if (customer.Segment == "Education" && plan.IsEducationEligible)
            {
                discountAmount += baseAmount * 0.20m;
                notes += "education discount; ";
            }

            if (customer.YearsWithCompany >= 5)
            {
                discountAmount += baseAmount * 0.07m;
                notes += "long-term loyalty discount; ";
            }
            else if (customer.YearsWithCompany >= 2)
            {
                discountAmount += baseAmount * 0.03m;
                notes += "basic loyalty discount; ";
            }

            if (seatCount >= 50)
            {
                discountAmount += baseAmount * 0.12m;
                notes += "large team discount; ";
            }
            else if (seatCount >= 20)
            {
                discountAmount += baseAmount * 0.08m;
                notes += "medium team discount; ";
            }
            else if (seatCount >= 10)
            {
                discountAmount += baseAmount * 0.04m;
                notes += "small team discount; ";
            }

            return discountAmount;
        }
    }
}