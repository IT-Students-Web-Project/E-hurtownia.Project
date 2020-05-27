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
    }
}
