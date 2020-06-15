using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace E_hurtownia.Models
{
    public partial class Storehouses
    {
        public Storehouses()
        {
            Stocks = new HashSet<Stocks>();
            Storekeepers = new HashSet<Storekeepers>();
        }

        public int IdStorehouse { get; set; }
        public int? FkAddress { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public virtual Addresses FkAddressNavigation { get; set; }
        public virtual ICollection<Stocks> Stocks { get; set; }
        public virtual ICollection<Storekeepers> Storekeepers { get; set; }
    }
}
