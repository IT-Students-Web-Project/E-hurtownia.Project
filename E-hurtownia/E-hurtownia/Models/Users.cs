using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Users
    {
        public Users()
        {
            Customers = new HashSet<Customers>();
            Storekeepers = new HashSet<Storekeepers>();
        }

        public int IdUser { get; set; }
        public int? FkGroup { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }

        public virtual Groups FkGroupNavigation { get; set; }
        public virtual ICollection<Customers> Customers { get; set; }
        public virtual ICollection<Storekeepers> Storekeepers { get; set; }
    }
}
