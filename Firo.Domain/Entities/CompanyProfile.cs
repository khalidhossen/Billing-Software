using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Entities
{
    public class CompanyProfile
    {
        [Key]
        public int Id { get; set; }
        public Guid CompanyProfileId { get; set; }
        public string? CompanyName { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string? BIN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoPath { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public bool IsActive { get; set; }


        public string? MailServer { get; set; }
        public int Port { get; set; }
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }


        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

}
