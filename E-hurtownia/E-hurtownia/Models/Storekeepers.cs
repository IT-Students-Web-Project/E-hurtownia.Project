using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Storekeepers
    {
        public int IdStorekeeper { get; set; }
        public int FkUser { get; set; }
        public int FkStorehouse { get; set; }

        public virtual Storehouses FkStorehouseNavigation { get; set; }
        public virtual Users FkUserNavigation { get; set; }
    }
}
