using Firo.Application.Models;

using Firo.Application.ViewModels;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace Firo.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<CustomerDto>> GetCustomersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Customers.AsQueryable();
            var totalItems = await query.CountAsync();

            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    CompanyProfileId = c.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                        .Where(cp => cp.CompanyProfileId == c.CompanyProfileId)
                        .Select(cp => cp.CompanyName)
                        .FirstOrDefault(),
                    BranchId = c.BranchId,
                    BranchName = _context.Branches
                        .Where(b => b.BranchId == c.BranchId)
                        .Select(b => b.BranchName)
                        .FirstOrDefault(),
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CustomerType = c.CustomerType,
                    OpeningDue = c.OpeningDue,
                    IsActive = c.IsActive,
                    Notes = c.Notes,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResult<CustomerDto>
            {
                Items = customers,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public async Task<CustomerDto?> GetByIdAsync(Guid customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,

                    CompanyProfileId = c.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                                      .Where(cp => cp.CompanyProfileId == c.CompanyProfileId)
                                      .Select(cp => cp.CompanyName)
                                      .FirstOrDefault(),

                    BranchId = c.BranchId,
                    BranchName = _context.Branches
                                      .Where(b => b.BranchId == c.BranchId)
                                      .Select(b => b.BranchName)
                                      .FirstOrDefault(),

                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CustomerType = c.CustomerType,
                    OpeningDue = c.OpeningDue,
                    IsActive = c.IsActive,
                    Notes = c.Notes,

                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CustomerDto>> SearchByPhoneAsync(string phone)
        {
            var details = await _context.Customers
                .Where(c => c.Phone.Contains(phone))
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Phone = c.Phone
                })
                .ToListAsync();
            return details;
        }

        public async Task<Customer?> GetByEmailOrPhoneAsync(string? email, string? phone)
        {
            return await _context.Customers
                .Where(c => c.Email == email || c.Phone == phone)
                .FirstOrDefaultAsync();
        }
        public async Task<CustomerDto> AddCustomerAsync(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),

                CompanyProfileId = customerDto.CompanyProfileId,
                BranchId = customerDto.BranchId,
                FullName = customerDto.FullName,
                Phone = customerDto.Phone,
                Email = customerDto.Email,
                Address = customerDto.Address,
                CustomerType = customerDto.CustomerType,
                OpeningDue = customerDto.OpeningDue,
                IsActive = customerDto.IsActive,
                Notes = customerDto.Notes,

                CreatedBy = customerDto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            customerDto.Id = customer.Id;
            customerDto.CustomerId = customer.CustomerId;

            return customerDto;
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerDto)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == customerDto.CustomerId)
                .FirstOrDefaultAsync();

            if (customer == null)
                throw new KeyNotFoundException("Customer not found.");

            customer.CompanyProfileId = customerDto.CompanyProfileId;
            customer.BranchId = customerDto.BranchId;
            customer.FullName = customerDto.FullName;
            customer.Phone = customerDto.Phone;
            customer.Email = customerDto.Email;
            customer.Address = customerDto.Address;
            customer.CustomerType = customerDto.CustomerType;
            customer.OpeningDue = customerDto.OpeningDue;
            customer.IsActive = customerDto.IsActive;
            customer.Notes = customerDto.Notes;

            customer.UpdatedBy = customerDto.UpdatedBy;
            customer.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return customerDto;
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .FirstOrDefaultAsync();

            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CustomerDto?> GetCustomerListByPhoneNumber(string phoneNumber)
        {
            return await _context.Customers
                .Where(c => c.Phone == phoneNumber)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,

                    CompanyProfileId = c.CompanyProfileId,
                    CompanyName = _context.CompanyProfiles
                                      .Where(cp => cp.CompanyProfileId == c.CompanyProfileId)
                                      .Select(cp => cp.CompanyName)
                                      .FirstOrDefault(),

                    BranchId = c.BranchId,
                    BranchName = _context.Branches
                                      .Where(b => b.BranchId == c.BranchId)
                                      .Select(b => b.BranchName)
                                      .FirstOrDefault(),

                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CustomerType = c.CustomerType,
                    OpeningDue = c.OpeningDue,
                    IsActive = c.IsActive,
                    Notes = c.Notes,

                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Customers.CountAsync();
        }
        public async Task<int> GetTodaysCustomersAsync()
        {
            var today = DateTime.Today;
            return await _context.Customers
                .Where(c => c.CreatedAt.Date == today)
                .CountAsync();
        }
        public async Task<List<int>> GetDailyCustomersLast7DaysAsync()
        {
            var today = DateTime.Today;
            var past7Days = Enumerable.Range(0, 7)
                                      .Select(i => today.AddDays(-i))
                                      .OrderBy(d => d)
                                      .ToList();

            var dailyCustomers = await _context.Customers
                .Where(c => c.CreatedAt >= today.AddDays(-6))
                .GroupBy(c => c.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return past7Days
                .Select(day => dailyCustomers.FirstOrDefault(x => x.Date == day)?.Count ?? 0)
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
        public async Task<List<int>> GetMonthlyCustomersLast6MonthsAsync()
        {
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            // Step 1: Get actual grouped customer count from DB
            var groupedData = await _context.Customers
                .Where(c => c.CreatedAt >= startDate)
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            // Step 2: Build full 6-month list
            var last6Months = GetLast6Months();

            // Step 3: Match grouped data with full list, and return zero if missing
            var result = last6Months
                .Select(m =>
                    groupedData.FirstOrDefault(g => g.Year == m.Year && g.Month == m.Month)?.Count ?? 0
                )
                .ToList();

            return result;
        }
    }
}
