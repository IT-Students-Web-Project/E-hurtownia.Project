using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class OrderItems
    {
        public int IdOrderItem { get; set; }
        public int FkOrder { get; set; }
        public int FkProduct { get; set; }
        public int Amount { get; set; }

        public virtual Orders FkOrderNavigation { get; set; }
        public virtual Products FkProductNavigation { get; set; }
    }
}