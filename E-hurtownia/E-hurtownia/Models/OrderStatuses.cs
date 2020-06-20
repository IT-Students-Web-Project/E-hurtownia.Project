using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class OrderStatuses
    {
        public OrderStatuses()
        {
            Orders = new HashSet<Orders>();
        }

        public int IdOrderStatus { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }

        public void Configure(EntityTypeBuilder<OrderStatuses> builder)
        {
            builder.HasData(
                new OrderStatuses { IdOrderStatus = 1, Name = "Started (unpaid)" },
                new OrderStatuses { IdOrderStatus = 2, Name = "Paid" },
                new OrderStatuses { IdOrderStatus = 3, Name = "Sent" },
                new OrderStatuses { IdOrderStatus = 4, Name = "Delivered" });
        }
    }
}
