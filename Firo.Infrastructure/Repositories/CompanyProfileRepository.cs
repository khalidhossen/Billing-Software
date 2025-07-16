using Firo.Application.Models;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Firo.Infrastructure.Repositories
{
    public class CompanyProfileRepository : ICompanyProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyProfileDto>> GetAllCompanyProfileAsync()
        {
            return await _context.CompanyProfiles
                .Select(cp => new CompanyProfileDto
                {
                    Id = cp.Id,
                    CompanyProfileId = cp.CompanyProfileId,

                    CompanyName = cp.CompanyName,
                    TradeLicenseNo = cp.TradeLicenseNo,
                    BIN = cp.BIN,
                    Address = cp.Address,
                    Phone = cp.Phone,
                    Email = cp.Email,
                    LogoPath = cp.LogoPath,
                    SubscriptionStartDate = cp.SubscriptionStartDate,
                    SubscriptionEndDate = cp.SubscriptionEndDate,
                    IsActive = cp.IsActive,

                    MailServer = cp.MailServer,
                    Port = cp.Port,
                    SenderName = cp.SenderName,
                    SenderEmail = cp.SenderEmail,
                    UserName = cp.UserName,
                    Password = cp.Password,
                    EnableSSL = cp.EnableSSL,

                    CreatedBy = cp.CreatedBy,
                    CreatedAt = cp.CreatedAt,
                    UpdatedBy = cp.UpdatedBy,
                    UpdatedAt = cp.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<CompanyProfileDto?> GetByIdAsync(Guid CompanyProfileId)
        {
            return await _context.CompanyProfiles
                .Where(cp => cp.CompanyProfileId == CompanyProfileId)
                .Select(cp => new CompanyProfileDto
                {
                    Id = cp.Id,
                    CompanyProfileId = cp.CompanyProfileId,

                    CompanyName = cp.CompanyName,
                    TradeLicenseNo = cp.TradeLicenseNo,
                    BIN = cp.BIN,
                    Address = cp.Address,
                    Phone = cp.Phone,
                    Email = cp.Email,
                    LogoPath = cp.LogoPath,
                    SubscriptionStartDate = cp.SubscriptionStartDate,
                    SubscriptionEndDate = cp.SubscriptionEndDate,
                    IsActive = cp.IsActive,

                    MailServer = cp.MailServer,
                    Port = cp.Port,
                    SenderName = cp.SenderName,
                    SenderEmail = cp.SenderEmail,
                    UserName = cp.UserName,
                    Password = cp.Password,
                    EnableSSL = cp.EnableSSL,

                    CreatedBy = cp.CreatedBy,
                    CreatedAt = cp.CreatedAt,
                    UpdatedBy = cp.UpdatedBy,
                    UpdatedAt = cp.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CompanyProfileDto> AddCompanyProfileAsync(CompanyProfileDto companyProfileDto)
        {
            var companyProfile = new CompanyProfile
            {
                CompanyProfileId = Guid.NewGuid(),

                CompanyName = companyProfileDto.CompanyName,
                TradeLicenseNo = companyProfileDto.TradeLicenseNo,
                BIN= companyProfileDto.BIN,
                Address = companyProfileDto.Address,
                Phone = companyProfileDto.Phone,
                Email = companyProfileDto.Email,
                LogoPath = companyProfileDto.LogoString,
                SubscriptionStartDate = companyProfileDto.SubscriptionStartDate,
                SubscriptionEndDate = companyProfileDto.SubscriptionEndDate,
                IsActive = companyProfileDto.IsActive,

                MailServer = companyProfileDto.MailServer,
                Port = companyProfileDto.Port,
                SenderName = companyProfileDto.SenderName,
                SenderEmail = companyProfileDto.SenderEmail,
                UserName = companyProfileDto.UserName,
                Password = companyProfileDto.Password,
                EnableSSL = companyProfileDto.EnableSSL,

                CreatedBy = companyProfileDto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.CompanyProfiles.Add(companyProfile);
            await _context.SaveChangesAsync();

            companyProfileDto.Id = companyProfile.Id;

            return companyProfileDto;
        }

        public async Task<CompanyProfileDto> UpdateCompanyProfileAsync(CompanyProfileDto companyProfileDto)
        {
            var companyProfile = await _context.CompanyProfiles
                .Where(x=>x.CompanyProfileId == companyProfileDto.CompanyProfileId)
                .FirstOrDefaultAsync();

            if (companyProfile == null) 
                throw new KeyNotFoundException("Company profile not found.");

            companyProfile.CompanyName = companyProfileDto.CompanyName;
            companyProfile.TradeLicenseNo = companyProfileDto.TradeLicenseNo;
            companyProfile.BIN = companyProfileDto.BIN;
            companyProfile.Address = companyProfileDto.Address;
            companyProfile.Phone = companyProfileDto.Phone;
            companyProfile.Email = companyProfileDto.Email;
            companyProfile.LogoPath = companyProfileDto.LogoString;
            companyProfile.SubscriptionStartDate = companyProfileDto.SubscriptionStartDate;
            companyProfile.SubscriptionEndDate = companyProfileDto.SubscriptionEndDate;
            companyProfile.IsActive = companyProfileDto.IsActive;

            companyProfile.MailServer = companyProfileDto.MailServer;
            companyProfile.Port = companyProfileDto.Port;
            companyProfile.SenderName = companyProfileDto.SenderName;
            companyProfile.SenderEmail = companyProfileDto.SenderEmail;
            companyProfile.UserName = companyProfileDto.UserName;
            companyProfile.Password = companyProfileDto.Password;
            companyProfile.EnableSSL = companyProfileDto.EnableSSL;

            companyProfile.UpdatedBy = companyProfileDto.UpdatedBy;
            companyProfile.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            companyProfileDto.LogoPath = companyProfile.LogoPath;
            return companyProfileDto;
        }

        public async Task<bool> DeleteCompanyProfileAsync(Guid CompanyProfileId)
        {
            var companyProfile = await _context.CompanyProfiles.Where(x => x.CompanyProfileId == CompanyProfileId).FirstOrDefaultAsync();

            if (companyProfile == null) return false;

            _context.CompanyProfiles.Remove(companyProfile);
            await _context.SaveChangesAsync();

            return true;
        }

    }

}
