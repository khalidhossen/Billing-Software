using Firo.Application.Models;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firo.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<InvoiceDto>> GetInvoicesPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Invoices.AsQueryable();
            var totalItems = await query.CountAsync();

            var invoices = await query
                .OrderBy(i => i.InvoiceNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceId = i.InvoiceId,

                    CompanyProfileId = i.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                        .Where(cp => cp.CompanyProfileId == i.CompanyProfileId)
                        .Select(cp => cp.CompanyName)
                        .FirstOrDefault(),

                    BranchId = i.BranchId,
                    BranchName = _context.Branches
                        .Where(b => b.BranchId == i.BranchId)
                        .Select(b => b.BranchName)
                        .FirstOrDefault(),

                    CustomerId = i.CustomerId,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.FullName)
                        .FirstOrDefault(),
                    CustomerPhone = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.Phone)
                        .FirstOrDefault(),

                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    Subtotal = i.Subtotal,
                    Discount = i.Discount,
                    ExtraDiscount = i.ExtraDiscount,
                    TotalDiscount = i.TotalDiscount,
                    GrandTotal = i.GrandTotal,
                    PayAmount = i.PayAmount,
                    DueAmount = i.DueAmount,
                    ExchangeAmount = i.ExchangeAmount,
                    Paid = i.Paid,
                    PaymentMethod = i.PaymentMethod,

                    CreatedBy = i.CreatedBy,
                    CreatedAt = i.CreatedAt,
                    UpdatedBy = i.UpdatedBy,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResult<InvoiceDto>
            {
                Items = invoices,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PaginatedResult<InvoiceDto>> GetDueInvoicesPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Invoices
                .Where(i => i.DueAmount > 0);

            var totalItems = await query.CountAsync();

            var invoices = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceId = i.InvoiceId,
                    CompanyProfileId = i.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                        .Where(cp => cp.CompanyProfileId == i.CompanyProfileId)
                        .Select(cp => cp.CompanyName)
                        .FirstOrDefault(),
                    BranchId = i.BranchId,
                    BranchName = _context.Branches
                        .Where(b => b.BranchId == i.BranchId)
                        .Select(b => b.BranchName)
                        .FirstOrDefault(),
                    CustomerId = i.CustomerId,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.FullName)
                        .FirstOrDefault(),
                    CustomerPhone = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.Phone)
                        .FirstOrDefault(),
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    Subtotal = i.Subtotal,
                    Discount = i.Discount,
                    ExtraDiscount = i.ExtraDiscount,
                    TotalDiscount = i.TotalDiscount,
                    GrandTotal = i.GrandTotal,
                    PayAmount = i.PayAmount,
                    DueAmount = i.DueAmount,
                    ExchangeAmount = i.ExchangeAmount,
                    Paid = i.Paid,
                    PaymentMethod = i.PaymentMethod,
                    CreatedBy = i.CreatedBy,
                    CreatedAt = i.CreatedAt,
                    UpdatedBy = i.UpdatedBy,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResult<InvoiceDto>
            {
                Items = invoices,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Invoice> GetInvoiceByIdAsync(Guid invoiceId)
        {
            return await _context.Invoices.FirstOrDefaultAsync(x => x.InvoiceId == invoiceId);
        }
        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
        public async Task<InvoiceDto?> GetByIdAsync(Guid invoiceId)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceId = i.InvoiceId,

					CompanyProfileId = i.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                        .Where(cp => cp.CompanyProfileId == i.CompanyProfileId)
                        .Select(cp => cp.CompanyName)
                        .FirstOrDefault(),

                    BranchId = i.BranchId,
                    BranchName = _context.Branches
                        .Where(b => b.BranchId == i.BranchId)
                        .Select(b => b.BranchName)
                        .FirstOrDefault(),

                    CustomerId = i.CustomerId,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.FullName)
                        .FirstOrDefault(),
                    
                    CustomerPhone = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.Phone)
                        .FirstOrDefault(),
                    CustomerAddress = _context.Customers
                        .Where(c => c.CustomerId == i.CustomerId)
                        .Select(c => c.Address)
                        .FirstOrDefault(),

                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    Subtotal = i.Subtotal,
                    Discount = i.Discount,
                    ExtraDiscount = i.ExtraDiscount,
                    TotalDiscount = i.TotalDiscount,
                    GrandTotal = i.GrandTotal,
                    PayAmount = i.PayAmount,
                    DueAmount = i.DueAmount,
                    ExchangeAmount = i.ExchangeAmount,
                    Paid = i.Paid,
                    PaymentMethod = i.PaymentMethod,
                    Notes = i.Notes,
                    Status = i.Status,

                    CreatedBy = i.CreatedBy,
                    CreatedAt = i.CreatedAt,
                    UpdatedBy = i.UpdatedBy,
                    UpdatedAt = i.UpdatedAt,

                    Products = _context.Products
                .Where(p => p.InvoiceId == i.InvoiceId)
                .Select(p => new ProductDto
                {
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Discount = p.Discount,
                    TotalPrice = p.TotalPrice
                }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetNextInvoiceNumberAsync()
        {
            var latestInvoiceNumber = await _context.Invoices
                .OrderByDescending(i => i.InvoiceNumber)
                .Select(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            return latestInvoiceNumber + 1;
        }
        public async Task<InvoiceDto> AddInvoiceAsync(InvoiceDto invoiceDto)
        {
            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                CompanyProfileId = invoiceDto.CompanyProfileId,
                BranchId = invoiceDto.BranchId,
                CustomerId = invoiceDto.CustomerId,
               
                InvoiceDate = invoiceDto.InvoiceDate,
                DueDate = invoiceDto.DueDate,
                InvoiceNumber = invoiceDto.InvoiceNumber,
                Subtotal = invoiceDto.Subtotal,
                Discount = invoiceDto.Discount,
                ExtraDiscount = invoiceDto.ExtraDiscount,
                TotalDiscount = invoiceDto.TotalDiscount,
               
                GrandTotal = invoiceDto.GrandTotal,
                PayAmount = invoiceDto.PayAmount,
                DueAmount = invoiceDto.DueAmount,
                ExchangeAmount = invoiceDto.ExchangeAmount,
                Paid = invoiceDto.Paid,
                PaymentMethod = invoiceDto.PaymentMethod,
                Notes = invoiceDto.Notes,
                Status = invoiceDto.Status,
                CreatedBy = invoiceDto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            invoiceDto.InvoiceId = invoice.InvoiceId;
            invoiceDto.Id = invoice.Id;
            invoiceDto.CreatedAt = invoice.CreatedAt;

            return invoiceDto;
        }

        public async Task<InvoiceDto> UpdateInvoiceAsync(InvoiceDto invoiceDto)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceDto.InvoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found.");

            invoice.CompanyProfileId = invoiceDto.CompanyProfileId;
            invoice.BranchId = invoiceDto.BranchId;
            invoice.CustomerId = invoiceDto.CustomerId;
            
            invoice.InvoiceDate = invoiceDto.InvoiceDate;
            invoice.DueDate = invoiceDto.DueDate;

            invoice.InvoiceNumber = invoiceDto.InvoiceNumber;
            invoice.Subtotal = invoiceDto.Subtotal;
            invoice.TotalDiscount = invoiceDto.TotalDiscount;
            invoice.Discount = invoiceDto.Discount;
            invoice.GrandTotal = invoiceDto.GrandTotal;
            invoice.PayAmount = invoiceDto.PayAmount;
            invoice.DueAmount = invoiceDto.DueAmount;
            invoice.ExchangeAmount = invoiceDto.ExchangeAmount;
            invoice.Paid = invoiceDto.Paid;
            invoice.PaymentMethod = invoiceDto.PaymentMethod;
            invoice.Notes = invoiceDto.Notes;
            invoice.Status = invoiceDto.Status;
            invoice.UpdatedBy = invoiceDto.UpdatedBy;
            invoice.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            invoiceDto.UpdatedAt = invoice.UpdatedAt;
            return invoiceDto;
        }

        public async Task<bool> DeleteInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null) return false;

            var products = await _context.Products
             .Where(p => p.InvoiceId == invoiceId)
             .ToListAsync();

            if (products.Any())
            {
                _context.Products.RemoveRange(products);
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<decimal> GetTotalSalesAmountAsync()
        {
            return await _context.Invoices.SumAsync(i => i.GrandTotal);
        }
        public async Task<decimal> GetTodaysSalesAmountAsync()
        {
            var today = DateTime.Today;
            return await _context.Invoices
                .Where(i => i.InvoiceDate.Date == today)
                .SumAsync(i => i.GrandTotal);
        }

        public async Task<decimal> GetTotalDueAmountAsync()
        {
            return await _context.Invoices
                .SumAsync(i => i.DueAmount); // Assumes 'DueAmount' field exists
        }

        public async Task<decimal> GetTodaysDueAmountAsync()
        {
            var today = DateTime.Today;
            return await _context.Invoices
                .Where(i => i.InvoiceDate.Date == today)
                .SumAsync(i => i.DueAmount); // Assumes 'DueAmount' field exists
        }
        public async Task<List<decimal>> GetDailySalesLast7DaysAsync()
        {
            var today = DateTime.Today;
            var past7Days = Enumerable.Range(0, 7)
                                      .Select(i => today.AddDays(-i))
                                      .OrderBy(d => d)
                                      .ToList();

            var dailySales = await _context.Invoices
                .Where(i => i.InvoiceDate >= today.AddDays(-6))
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(i => i.GrandTotal)
                })
                .ToListAsync();

            return past7Days
                .Select(day => dailySales.FirstOrDefault(x => x.Date == day)?.Total ?? 0)
                .ToList();
        }
        private List<(int Year, int Month)> GetLast6Months()
        {
            var months = new List<(int Year, int Month)>();
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Today.AddMonths(-i);
                months.Add((date.Year, date.Month));
            }
            return months;
        }
        public async Task<List<decimal>> GetMonthlySalesLast6MonthsAsync()
        {
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            var groupedData = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate)
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(i => i.GrandTotal)
                })
                .ToListAsync();

            var last6Months = GetLast6Months();

            var result = last6Months
                .Select(m =>
                    groupedData.FirstOrDefault(g => g.Year == m.Year && g.Month == m.Month)?.Total ?? 0
                )
                .ToList();

            return result;
        }
    }
}
