using System;

namespace Mozaika.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Article { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public decimal MinPartnerPrice { get; set; }
        public decimal PackageLength { get; set; }
        public decimal PackageWidth { get; set; }
        public decimal PackageHeight { get; set; }
        public decimal WeightWithoutPackage { get; set; }
        public decimal WeightWithPackage { get; set; }
        public byte[]? QualityCertificate { get; set; }
        public string StandardNumber { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public int WorkshopNumber { get; set; }
        public int ProductionWorkersCount { get; set; }
        public TimeSpan ProductionTime { get; set; }
        public string RequiredMaterials { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}





