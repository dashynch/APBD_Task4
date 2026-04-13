namespace LegacyRenewalApp.Interfaces
{

    public interface ISubscriptionPlanRepository
    {
        SubscriptionPlan GetByCode(string planCode);
    }
}