using Firo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Interfaces
{
    public interface ILookUpRepository
    {
        Task<IEnumerable<LookUpDto>> GetAllLookUpAsync();
        Task<LookUpDto?> GetByIdAsync(Guid LookUpId);
        Task<List<LookUpDto>> GetByDataKeyAsync(string dataKey);
        Task<List<LookUpDto>> GetByDataKeysAsync(List<string> dataKeys);
        Task<LookUpDto> AddLookUpAsync(LookUpDto lookUpDto);
        Task<LookUpDto> UpdateLookUpAsync(LookUpDto lookUpDto);
        Task<bool> DeleteLookUpAsync(Guid LookUpIdd);
    }
}
