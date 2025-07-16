using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid CustomerId { get; set; }

        public string? ProductName { get; set; }
        

        public string? Category { get; set; }

        public decimal Quantity { get; set; }
        
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalPrice { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
