using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstantTalk.Server.Models
{
    public class Bill
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Subscriber")]
        public Guid SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public bool IsPaid { get; set; } = false;

        [Required]
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }
}
