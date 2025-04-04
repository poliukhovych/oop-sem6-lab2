using System.ComponentModel.DataAnnotations;

namespace ConstantTalk.Server.Models
{
    public class Service
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public List<SubscriberService>? SubscriberServices { get; set; }
    }
}
