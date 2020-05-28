using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Companies
    {
        public Companies()
        {
            Customers = new HashSet<Customers>();
        }

        public int IdComapny { get; set; }
        public string Name { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public int FkAddress { get; set; }

        public virtual Addresses FkAddressNavigation { get; set; }
        public virtual ICollection<Customers> Customers { get; set; }
    }
}
