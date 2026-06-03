namespace GLMS.DTOs
{
    public class ClientDto
    {
        public int ClientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;
    }
}