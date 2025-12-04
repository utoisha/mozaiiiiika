using System;
using System.Collections.Generic;

namespace Mozaika.Models
{
    public enum OrderStatus
    {
        Created,
        PrepaymentPending,
        PrepaymentReceived,
        InProduction,
        ReadyForDelivery,
        Delivered,
        Completed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PrepaymentDeadline { get; set; }
        public DateTime? PrepaymentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Created;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ProductionDate { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}





