using Firo.Application.Models;
using Firo.Application.ViewModels;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Infrastructure.Repositories
{
    public class PayDueRepository: IPayDueRepository
    {
        private readonly ApplicationDbContext _context;

        public PayDueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<PayDueDto>> GetAllAsync(int pageNumber,int pageSize)
        {
            var query = _context.PayDues.AsQueryable();
            var totalItems = await query.CountAsync();
            var pays = await query
                .OrderByDescending(i => i.PayDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PayDueDto
                {
                    Id = p.Id,
                    PayDueId = p.PayDueId,
                    InvoiceId = p.InvoiceId,
                    CustomerId = p.CustomerId,
                    CurrentPay = p.CurrentPay,
                    CurrentDue = p.CurrentDue,
                    PayDate = p.PayDate,
                    CreatedBy = p.CreatedBy,
                    CreatedAt = p.CreatedAt,
                    UpdatedBy = p.UpdatedBy,
                    UpdatedAt = p.UpdatedAt,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.FullName)
                        .FirstOrDefault(),
                    InvoiceNumber = _context.Invoices
                        .Where(i => i.InvoiceId == p.InvoiceId)
                        .Select(i => i.InvoiceNumber)
                        .FirstOrDefault()
                })
                .ToListAsync();
            return new PaginatedResult<PayDueDto>
            {
                Items = pays,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PayDueDto> CreateAsync(PayDueDto dto)
        {
            var entity = new PayDue
            {
                PayDueId = Guid.NewGuid(),
                InvoiceId = dto.InvoiceId,
                CustomerId = dto.CustomerId,
                CurrentPay = dto.CurrentPay,
                CurrentDue = dto.CurrentDue,
                PayDate = dto.PayDate,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                UpdatedBy = dto.UpdatedBy,
                UpdatedAt = DateTime.Now
            };

            _context.PayDues.Add(entity); 
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.PayDueId = entity.PayDueId;
            dto.CreatedAt = entity.CreatedAt;
            dto.UpdatedAt = entity.UpdatedAt;

            return dto;
        }
    }
}
