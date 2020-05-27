using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Customers
    {
        public Customers()
        {
            Orders = new HashSet<Orders>();
        }

        public int IdCustomer { get; set; }
        public int? FkPerson { get; set; }
        public int? FkCompany { get; set; }
        public int FkUser { get; set; }
        public bool Status { get; set; }

        public virtual Companies FkCompanyNavigation { get; set; }
        public virtual Persons FkPersonNavigation { get; set; }
        public virtual Users FkUserNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
