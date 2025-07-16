using Firo.Application.Models;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firo.Infrastructure.Repositories
{
    public class LookUpRepository : ILookUpRepository
    {
        private readonly ApplicationDbContext _context;

        public LookUpRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LookUpDto>> GetAllLookUpAsync()
        {
            return await _context.LookUps
                .Select(l => new LookUpDto
                {
                    Id = l.Id,
                    LookUpId = l.LookUpId,
                    DataKey = l.DataKey,
                    DisplayText = l.DisplayText,
                    DataValue = l.DataValue,
                    DataOrder = l.DataOrder,
                    IsActive = l.IsActive
                })
                .ToListAsync();
        }

        public async Task<LookUpDto?> GetByIdAsync(Guid LookUpId)
        {
            return await _context.LookUps
                .Where(l => l.LookUpId == LookUpId)
                .Select(l => new LookUpDto
                {
                    Id = l.Id,
                    LookUpId = l.LookUpId,
                    DataKey = l.DataKey,
                    DisplayText = l.DisplayText,
                    DataValue = l.DataValue,
                    DataOrder = l.DataOrder,
                    IsActive = l.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<LookUpDto>> GetByDataKeyAsync(string dataKey)
        {
            return await _context.LookUps
                .Where(l => l.DataKey == dataKey)
                .Select(l => new LookUpDto
                {
                    Id = l.Id,
                    LookUpId = l.LookUpId,
                    DataKey = l.DataKey,
                    DisplayText = l.DisplayText,
                    DataValue = l.DataValue,
                    DataOrder = l.DataOrder,
                    IsActive = l.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<LookUpDto>> GetByDataKeysAsync(List<string> dataKeys)
        {
            return await _context.LookUps
                .Where(l => dataKeys.Contains(l.DataKey) && l.IsActive)
                .OrderBy(l => l.DataOrder)
                .Select(l => new LookUpDto
                {
                    Id = l.Id,
                    LookUpId = l.LookUpId,
                    DataKey = l.DataKey,
                    DisplayText = l.DisplayText,
                    DataValue = l.DataValue,
                    DataOrder = l.DataOrder,
                    IsActive = l.IsActive
                })
                .ToListAsync();
        }

        public async Task<LookUpDto> AddLookUpAsync(LookUpDto lookUpDto)
        {
            var lookUp = new LookUp
            {
                LookUpId = Guid.NewGuid(),
                DataKey = lookUpDto.DataKey,
                DisplayText = lookUpDto.DisplayText,
                DataValue = lookUpDto.DataValue,
                DataOrder = lookUpDto.DataOrder,
                IsActive = lookUpDto.IsActive
            };

            _context.LookUps.Add(lookUp);
            await _context.SaveChangesAsync();

            lookUpDto.Id = lookUp.Id;
            lookUpDto.LookUpId = lookUp.LookUpId;

            return lookUpDto;
        }

        public async Task<LookUpDto> UpdateLookUpAsync(LookUpDto lookUpDto)
        {
            var lookUp = await _context.LookUps.FirstOrDefaultAsync(l => l.LookUpId == lookUpDto.LookUpId);
            if (lookUp == null) throw new KeyNotFoundException("LookUp not found.");

            lookUp.DataKey = lookUpDto.DataKey;
            lookUp.DisplayText = lookUpDto.DisplayText;
            lookUp.DataValue = lookUpDto.DataValue;
            lookUp.DataOrder = lookUpDto.DataOrder;
            lookUp.IsActive = lookUpDto.IsActive;

            await _context.SaveChangesAsync();
            return lookUpDto;
        }

        public async Task<bool> DeleteLookUpAsync(Guid LookUpId)
        {
           

            var lookUp = await _context.LookUps.FirstOrDefaultAsync(l => l.LookUpId == LookUpId);
            if (lookUp == null) return false;

            _context.LookUps.Remove(lookUp);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

