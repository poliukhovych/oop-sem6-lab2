using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstantTalk.Server.Models
{
    public class SubscriberService
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Subscriber")]
        public Guid SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
