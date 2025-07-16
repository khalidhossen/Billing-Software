using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Application.ViewModels
{
    public class InvoiceVM
    {
        public string? CustomerFullName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public List<InvoiceProductVM> Products { get; set; }
        public int InvoiceNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal  Discount{ get; set; }
        public decimal ExtraDiscount { get; set; } 
        public decimal GrandTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal PayAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ExchangeAmount { get; set; }
        public decimal Paid { get; set; }
        public string? PaymentMethod { get; set; }






    }

    public class InvoiceProductVM
    {
        public string? ProductName { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
