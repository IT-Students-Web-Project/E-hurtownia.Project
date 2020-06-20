using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Addresses
    {
        public Addresses()
        {
            Companies = new HashSet<Companies>();
            Persons = new HashSet<Persons>();
            Storehouses = new HashSet<Storehouses>();
        }

        public int IdAddress { get; set; }
        public string Street { get; set; }
        public int BuildingNum { get; set; }
        public int? ApartmentNum { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Companies> Companies { get; set; }
        public virtual ICollection<Persons> Persons { get; set; }
        public virtual ICollection<Storehouses> Storehouses { get; set; }
    }
}
