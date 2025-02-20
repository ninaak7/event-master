namespace EMS.Domain.Models
{
    public class StripeSettings : BaseEntity
    {
        public string PublishableKey { get; set; }
        public string SecretKey { get; set; }
    }
}

