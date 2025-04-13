namespace ConstantTalk.Server.Models.DTOs
{
    public class BillDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}