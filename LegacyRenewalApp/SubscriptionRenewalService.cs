using System;
using LegacyRenewalApp.Helper;
using LegacyRenewalApp.Interfaces;
using LegacyRenewalApp.Repositories;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IBillingGateway _billingGateway;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ITaxCalculator _taxCalculator;
        private readonly IFeeCalculator _feeCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IRenewalInvoiceValidator _renewalInvoiceValidator;
        
        public SubscriptionRenewalService() : this(new CustomerRepository(), new SubscriptionPlanRepository(), new BillingGatewayAdapter(), new RenewalInvoiceValidator(), new DiscountCalculator(), new TaxCalculator(), new FeeCalculator(), new SupportFeeCalculator())
        {
            
        }
        
        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository planRepository, IBillingGateway billingGateway, IRenewalInvoiceValidator renewalInvoiceValidator, IDiscountCalculator discountCalculator, ITaxCalculator taxCalculator, IFeeCalculator feeCalculator, ISupportFeeCalculator supportFeeCalculator)
        { 
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _billingGateway = billingGateway;
            _renewalInvoiceValidator = renewalInvoiceValidator;
            _discountCalculator = discountCalculator;
            _taxCalculator = taxCalculator;
            _feeCalculator = feeCalculator;
            _supportFeeCalculator = supportFeeCalculator;
        }
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _renewalInvoiceValidator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();
            
            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
            decimal discountAmount = 0m;
            string notes = string.Empty;

            discountAmount = _discountCalculator.CalculateDiscount(customer, plan, seatCount, baseAmount, out string discountNotes);
            notes += discountNotes;
            
            if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
            {
                int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
                discountAmount += pointsToUse;
                notes += $"loyalty points used: {pointsToUse}; ";
            }

            decimal subtotalAfterDiscount = baseAmount - discountAmount;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            decimal supportFee = _supportFeeCalculator.CalculateFee(normalizedPlanCode, includePremiumSupport, out string supportNote);
            notes += supportNote;
            
            decimal paymentFee = _feeCalculator.CalculateFee(subtotalAfterDiscount, supportFee, normalizedPaymentMethod, out string feeNote);
            notes += feeNote;
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = _taxCalculator.CalculateTax(taxBase, customer);
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                _billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
