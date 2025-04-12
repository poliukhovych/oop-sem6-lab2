namespace ConstantTalk.Server.Models.DTOs 
{
    public class AddServiceRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}