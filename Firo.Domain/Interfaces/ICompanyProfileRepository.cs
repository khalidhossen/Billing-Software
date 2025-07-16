using Firo.Application.Models;
using Firo.Domain.Entities;

namespace Firo.Domain.Interfaces
{
    public interface ICompanyProfileRepository
    {
        Task<IEnumerable<CompanyProfileDto>> GetAllCompanyProfileAsync();
        Task<CompanyProfileDto?> GetByIdAsync(Guid CompanyProfileId);
        Task<CompanyProfileDto> AddCompanyProfileAsync(CompanyProfileDto companyProfileDto);
        Task<CompanyProfileDto> UpdateCompanyProfileAsync(CompanyProfileDto companyProfileDto);
        Task<bool> DeleteCompanyProfileAsync(Guid CompanyProfileId);
    }
}
