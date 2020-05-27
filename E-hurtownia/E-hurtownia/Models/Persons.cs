using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Persons
    {
        public Persons()
        {
            Customers = new HashSet<Customers>();
        }

        public int IdPerson { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Sex { get; set; }
        public int FkAddress { get; set; }

        public virtual Addresses FkAddressNavigation { get; set; }
        public virtual ICollection<Customers> Customers { get; set; }
    }
}
