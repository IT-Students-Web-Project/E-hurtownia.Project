using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Stocks
    {
        public int IdStock { get; set; }
        public int FkProduct { get; set; }
        public int Amount { get; set; }
        public int FkStorehouse { get; set; }
        public bool Status { get; set; }

        public virtual Products FkProductNavigation { get; set; }
        public virtual Storehouses FkStorehouseNavigation { get; set; }
    }
}
