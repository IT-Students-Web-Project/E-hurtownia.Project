using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace E_hurtownia.Models
{
    public partial class Rights
    {
        public Rights()
        {
            RightsAssignments = new HashSet<RightsAssignments>();
        }

        public int IdRight { get; set; }
        public int FkObject { get; set; }
        public string Description { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public virtual Objects FkObjectNavigation { get; set; }
        public virtual ICollection<RightsAssignments> RightsAssignments { get; set; }
    }
}
