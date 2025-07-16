using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Application.ViewModels
{
    public class PayDueVM
    {
        public int InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime PayDate { get; set; }
        public decimal DueAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UpdatedDueAmount { get; set; }
        public decimal PayAmount { get; set; }
    }
}
