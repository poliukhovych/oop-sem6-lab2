namespace ConstantTalk.Server.Models.DTOs
{
    public class AddUserRequest
    {
        public string Auth0Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
