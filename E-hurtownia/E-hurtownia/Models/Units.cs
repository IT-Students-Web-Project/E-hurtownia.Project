using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace E_hurtownia.Models
{
    public partial class Units
    {
        public Units()
        {
            Products = new HashSet<Products>();
        }

        [DisplayName("Unit ID")]
        public int IdUnit { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
