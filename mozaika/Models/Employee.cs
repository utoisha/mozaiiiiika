using System;

namespace Mozaika.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string PassportData { get; set; } = string.Empty;
        public string BankDetails { get; set; } = string.Empty;
        public bool HasFamily { get; set; }
        public string HealthStatus { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.Now;
        public decimal Salary { get; set; }
        public string EquipmentAccess { get; set; } = string.Empty; // Доступ к оборудованию
    }
}





