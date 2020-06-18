using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace E_hurtownia.Models
{
    public partial class Orders
    {
        public int IdOrder { get; set; }
        public int FkCustomer { get; set; }
        public int FkProduct { get; set; }
        public int Amount { get; set; }
        public DateTime? DateOrdered { get; set; }
        public DateTime? DateSent { get; set; }
        public DateTime? DatePaid { get; set; }
        public int FkOrderStatus { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public virtual Customers FkCustomerNavigation { get; set; }
        public virtual OrderStatuses FkOrderStatusNavigation { get; set; }
        public virtual Products FkProductNavigation { get; set; }
    }
}
