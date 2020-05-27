using System;
using System.Collections.Generic;

namespace E_hurtownia.Models
{
    public partial class Products
    {
        public Products()
        {
            Orders = new HashSet<Orders>();
            Stocks = new HashSet<Stocks>();
        }

        public int IdProduct { get; set; }
        public string Name { get; set; }
        public decimal BasePricePerUnit { get; set; }
        public string ImgFile { get; set; }
        public string PdfFile { get; set; }
        public int FkUnit { get; set; }

        public virtual Units FkUnitNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Stocks> Stocks { get; set; }
    }
}
