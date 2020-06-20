using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderItems = new HashSet<OrderItems>();
        }

        public int IdOrder { get; set; }
        public int FkCustomer { get; set; }
        public DateTime? DateOrdered { get; set; }
        public DateTime? DateSent { get; set; }
        public DateTime? DatePaid { get; set; }
        public int FkOrderStatus { get; set; }
        public bool Status { get; set; }

        public virtual Customers FkCustomerNavigation { get; set; }
        public virtual OrderStatuses FkOrderStatusNavigation { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}
