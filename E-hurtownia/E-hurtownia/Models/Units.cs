using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Units
    {
        public Units()
        {
            Products = new HashSet<Products>();
        }

        public int IdUnit { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
