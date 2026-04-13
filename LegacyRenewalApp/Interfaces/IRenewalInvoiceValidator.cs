namespace LegacyRenewalApp.Interfaces
{

    public interface IRenewalInvoiceValidator
    {
        void Validate(int customerId, string planCode, int seatCount, string paymentMethod);
    }
}