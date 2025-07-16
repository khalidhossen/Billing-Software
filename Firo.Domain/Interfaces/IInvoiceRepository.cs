using Firo.Application.Models;
using Firo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<PaginatedResult<InvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<InvoiceDto>> GetDueInvoicesPagedAsync(int pageNumber, int pageSize);
        Task<int> GetNextInvoiceNumberAsync();
        Task<InvoiceDto?> GetByIdAsync(Guid invoiceId);
        Task<Invoice> GetInvoiceByIdAsync(Guid invoiceId);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task<InvoiceDto> AddInvoiceAsync(InvoiceDto invoiceDto);
        Task<InvoiceDto> UpdateInvoiceAsync(InvoiceDto invoiceDto);
        Task<bool> DeleteInvoiceAsync(Guid invoiceId);
        Task<decimal> GetTodaysSalesAmountAsync();
        Task<List<decimal>> GetDailySalesLast7DaysAsync();
        Task<decimal> GetTotalDueAmountAsync();
        Task<decimal> GetTodaysDueAmountAsync();
        Task<decimal> GetTotalSalesAmountAsync();
        Task<List<decimal>> GetMonthlySalesLast6MonthsAsync();
    }
}
