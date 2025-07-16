using Firo.Application.Models;
using Firo.Domain.Entities;

namespace Firo.Domain.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<BranchDto>> GetAllBranchAsync();
        Task<BranchDto?> GetByIdAsync(Guid BranchId);
        Task<BranchDto> AddBranchAsync(BranchDto branchDto);
        Task<BranchDto> UpdateBranchAsync(BranchDto branchDto);
        Task<bool> DeleteBranchAsync(Guid BranchId);
    }
}
