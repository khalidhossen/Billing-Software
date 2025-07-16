using Firo.Application.Models;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Firo.Infrastructure.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranchAsync()
        {
            return await _context.Branches
                .Select(b => new BranchDto
                {
                    Id = b.Id,
                    BranchId = b.BranchId,

                    CompanyProfileId = b.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                                    .Where(cp => cp.CompanyProfileId == b.CompanyProfileId)
                                    .Select(cp => cp.CompanyName)
                                    .FirstOrDefault(),
                    BranchName = b.BranchName,
                    Location = b.Location,
                    Phone = b.Phone,
                    Email = b.Email,
                    IsMainBranch = b.IsMainBranch,
                    IsActive = b.IsActive,

                    CreatedBy = b.CreatedBy,
                    CreatedAt = b.CreatedAt,
                    UpdatedBy = b.UpdatedBy,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<BranchDto?> GetByIdAsync(Guid BranchId)
        {
            return await _context.Branches
                .Where(b => b.BranchId == BranchId)
                .Select(b => new BranchDto
                {
                    Id = b.Id,
                    BranchId = b.BranchId,

                    CompanyProfileId = b.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                                    .Where(cp => cp.CompanyProfileId == b.CompanyProfileId)
                                    .Select(cp => cp.CompanyName)
                                    .FirstOrDefault(),
                    BranchName = b.BranchName,
                    Location = b.Location,
                    Phone = b.Phone,
                    Email = b.Email,
                    IsMainBranch = b.IsMainBranch,
                    IsActive = b.IsActive,

                    CreatedBy = b.CreatedBy,
                    CreatedAt = b.CreatedAt,
                    UpdatedBy = b.UpdatedBy,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BranchDto> AddBranchAsync(BranchDto branchDto)
        {
            var branch = new Branch
            {
                BranchId = Guid.NewGuid(),

                CompanyProfileId = branchDto.CompanyProfileId,
                BranchName = branchDto.BranchName,
                Location = branchDto.Location,
                Phone = branchDto.Phone,
                Email = branchDto.Email,
                IsMainBranch = branchDto.IsMainBranch,
                IsActive = branchDto.IsActive,

                CreatedBy = branchDto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            branchDto.Id = branch.Id;

            return branchDto;
        }

        public async Task<BranchDto> UpdateBranchAsync(BranchDto branchDto)
        {
            var branch = await _context.Branches
                .Where(x => x.BranchId == branchDto.BranchId)
                .FirstOrDefaultAsync();

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            branch.CompanyProfileId = branchDto.CompanyProfileId;
            branch.BranchName = branchDto.BranchName;
            branch.Location = branchDto.Location;
            branch.Phone = branchDto.Phone;
            branch.Email = branchDto.Email;
            branch.IsMainBranch = branchDto.IsMainBranch;
            branch.IsActive = branchDto.IsActive;

            branch.UpdatedBy = branchDto.UpdatedBy;
            branch.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return branchDto;
        }

        public async Task<bool> DeleteBranchAsync(Guid BranchId)
        {
            var branch = await _context.Branches.Where(x => x.BranchId == BranchId).FirstOrDefaultAsync();

            if (branch == null) return false;

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}