using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Objects
    {
        public Objects()
        {
            Rights = new HashSet<Rights>();
        }

        public int IdObject { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Rights> Rights { get; set; }
    }
}
