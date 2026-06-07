namespace GLMS.DTOs
{
    public class ContractDto
    {
        public int ContractId { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? SignedAgreementFilePath { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}