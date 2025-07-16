using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Application.Models
{
    public class InvoiceDto
    {
        public int Id { get; set; }
		public Guid InvoiceId { get; set; }

		public Guid CompanyProfileId { get; set; }
        public string? CompanyName { get; set; } 

        public Guid BranchId { get; set; }
        public string? BranchName { get; set; } 

        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; } 
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public int InvoiceNumber { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal ExtraDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PayAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ExchangeAmount { get; set; }
        public decimal Paid { get; set; }
        public string? PaymentMethod { get; set; }
        public List<ProductDto> Products { get; set; }

        public string? Notes { get; set; }
        public string? Status { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
