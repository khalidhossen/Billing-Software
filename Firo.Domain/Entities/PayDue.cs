using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class PayDue
    {
        [Key]
        public int Id { get; set; }                
        public Guid PayDueId { get; set; }            
        public Guid InvoiceId { get; set; }        
        public Guid CustomerId { get; set; }       
        
        public decimal CurrentPay { get; set; }     
        public decimal CurrentDue { get; set; }             
        public DateTime PayDate { get; set; } = DateTime.Now;

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
