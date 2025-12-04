using System;

namespace Mozaika.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int PackageQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public decimal Cost { get; set; }
        public int StockQuantity { get; set; }
        public int MinQuantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}





