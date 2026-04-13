namespace LegacyRenewalApp.Interfaces
{

    public interface IDiscountCalculator
    {
        decimal CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, out string notes);
        string Description { get; }
    }
}