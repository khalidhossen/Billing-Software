using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Common.Helper
{
    public class Role
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Sales = "Sales";
        public const string Managment = "Management";
    }
    public class Helper
    {
        public const string Prefix = "6";
    }

    public class UploadFilePath
    {
        public const string ProductImage = "Image/Products";
        public const string CompanyLogo = "Image/logo";
        public const string Quotation = "wwwroot\\Files\\Quotation";
        public const string Invoices = "wwwroot\\Files\\Invoices";
        public const string Barcode = "wwwroot\\Files\\Barcodes";
    }
    public class Pagination
    {
        public const int PageSize = 10;
        public const int Take = 15;
    }
}
