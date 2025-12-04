using System;

namespace Mozaika.Models
{
    public class Partner
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string LegalAddress { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string DirectorName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Logo { get; set; }
        public int Rating { get; set; }
        public string SalesLocations { get; set; } = string.Empty;
        public decimal TotalSalesVolume { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}





