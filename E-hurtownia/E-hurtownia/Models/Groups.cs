using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Groups
    {
        public Groups()
        {
            RightsAssignments = new HashSet<RightsAssignments>();
            Users = new HashSet<Users>();
        }

        public int IdGroup { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<RightsAssignments> RightsAssignments { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
