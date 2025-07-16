using Firo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Interfaces
{
    public interface IPayDueRepository
    {
        Task<PaginatedResult<PayDueDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PayDueDto> CreateAsync(PayDueDto dto);
    }
}
