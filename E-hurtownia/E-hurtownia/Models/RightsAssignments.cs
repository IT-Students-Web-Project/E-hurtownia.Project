using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class RightsAssignments
    {
        public int IdRightAssignment { get; set; }
        public int FkGroup { get; set; }
        public int FkRight { get; set; }

        public virtual Groups FkGroupNavigation { get; set; }
        public virtual Rights FkRightNavigation { get; set; }
    }
}
