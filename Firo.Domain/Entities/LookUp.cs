using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class LookUp
    {
        [Key]
        public int Id { get; set; }
        public Guid LookUpId { get; set; }
        public string? DataKey { get; set; }
        public string? DisplayText { get; set; }
        public string? DataValue { get; set; }
        public string? DataOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
