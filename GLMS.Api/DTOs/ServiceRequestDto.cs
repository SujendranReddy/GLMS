namespace GLMS.DTOs
{
    public class ServiceRequestDto
    {
        public int ServiceRequestId { get; set; }

        public int ContractId { get; set; }

        public string ContractDescription { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}