using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace E_hurtownia.Models
{
    public partial class Products
    {
        public Products()
        {
            Orders = new HashSet<Orders>();
            Stocks = new HashSet<Stocks>();
        }

        [DisplayName("ID Product")]
        public int IdProduct { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Price per unit in euro")]
        public decimal BasePricePerUnit { get; set; }

        public string ImgFile { get; set; }

        public string PdfFile { get; set; }

        [DisplayName("Unit")]
        public int FkUnit { get; set; }

        [DisplayName("Unit")]
        [ForeignKey(nameof(FkUnit))]
        public virtual Units FkUnitNavigation { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }

        public virtual ICollection<Stocks> Stocks { get; set; }

        internal void DeleteImageFile()
        {
            DeleteFile(ImgFile);
        }

        internal void DeletePdfFile()
        {
            DeleteFile(PdfFile);
        }

        private void DeleteFile(string path)
        {
            string serverPath = GetServerPath(path);
            if (path != null && File.Exists(serverPath))
                File.Delete(serverPath);
        }

        public string GetServerPath(string remotePath)
        {
            string serverPath = Directory.GetCurrentDirectory() + "\\wwwroot" + remotePath;
            return serverPath;
        }
    }
}
