using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public Guid CustomerId { get; set; }

        public Guid CompanyProfileId { get; set; }
        public Guid BranchId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? CustomerType { get; set; }
        public double OpeningDue { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
