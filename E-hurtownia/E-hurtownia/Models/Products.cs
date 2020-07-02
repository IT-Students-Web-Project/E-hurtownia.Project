using System;
using System.Collections.Generic;
using System.IO;

namespace E_hurtownia.Models
{
    public partial class Products
    {
        public Products()
        {
            OrderItems = new HashSet<OrderItems>();
            Stocks = new HashSet<Stocks>();
        }

        public int IdProduct { get; set; }
        public string Name { get; set; }
        public decimal BasePricePerUnit { get; set; }
        public int FkUnit { get; set; }
        public string ImgFile { get; set; }
        public string PdfFile { get; set; }

        public virtual Units FkUnitNavigation { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
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
