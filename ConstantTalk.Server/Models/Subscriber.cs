using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstantTalk.Server.Models
{
    public class Subscriber
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Auth0Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsBlocked { get; set; } = false;

        public List<SubscriberService>? SubscriberServices { get; set; }
        public List<Bill>? Bills { get; set; }
    }
}
