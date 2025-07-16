using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid CompanyProfileId { get; set; }
        public string? BranchName { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsMainBranch { get; set; }
        public bool IsActive { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
