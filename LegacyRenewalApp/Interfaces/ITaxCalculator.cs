namespace LegacyRenewalApp.Interfaces
{

    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal taxBase, Customer customer);
    }
}