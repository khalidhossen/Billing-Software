using Firo.Application.Models;

using Firo.Domain.Entities;

namespace Firo.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<PaginatedResult<CustomerDto>> GetCustomersPagedAsync(int pageNumber, int pageSize);
        Task<CustomerDto?> GetByIdAsync(Guid customerId);
        Task<CustomerDto> AddCustomerAsync(CustomerDto customerDto);
        Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerDto);
        Task<bool> DeleteCustomerAsync(Guid customerId);
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTodaysCustomersAsync();
        Task<List<int>> GetDailyCustomersLast7DaysAsync();
        Task<List<int>> GetMonthlyCustomersLast6MonthsAsync();

        Task<CustomerDto?> GetCustomerListByPhoneNumber(string phoneNumber);

        Task<Customer?> GetByEmailOrPhoneAsync(string? email, string? phone);
        Task<IEnumerable<CustomerDto>> SearchByPhoneAsync(string phone);
    }

}
