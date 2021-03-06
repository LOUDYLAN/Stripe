using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stripe.Models
{
    public class CustomerPaymentViewModel
    {
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"\d{3}", ErrorMessage = "Invalid CVC number")]
        public string Cvc { get; set; }

        [Range(1, 12, ErrorMessage = "Invalid month")]
        public int ExpiryMonth { get; set; }

        [Range(17, 30, ErrorMessage = "Invalid year")]
        public int ExpiryYear { get; set; }
        
        // Donation attributes
        public int DonationId { get; set; }
        public string CycleId { get; set; }

        public List<CustomerSubscriptionViewModel> Subscriptions { get; set; }

        // Address

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        public string Frequency { get; set; }

        public string Description { get; set; }

        public int Amount { get; set; }
    }
}