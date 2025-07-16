using Firo.Application.Models;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firo.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Get All Products ---
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    InvoiceId = p.InvoiceId,
                    CustomerId = p.CustomerId,
                    ProductName = p.ProductName,
                   
                    Category = p.Category,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Discount = p.Discount,
                    TotalPrice = p.TotalPrice
                })
                .ToListAsync();
        }

        // --- Get Product by Id ---
        public async Task<ProductDto?> GetByIdAsync(Guid productId)
        {

            try
            {
                var result = await _context.Products
    .Where(p => p.ProductId == productId)
    .Select(p => new ProductDto
    {
        Id = p.Id,
        ProductId = p.ProductId,
        InvoiceId = p.InvoiceId,
        CustomerId = p.CustomerId,
        ProductName = p.ProductName,
        
        Category = p.Category,
        Quantity = p.Quantity,
        Price = p.Price,
        Discount = p.Discount,
        TotalPrice = p.TotalPrice,
        CreatedBy = p.CreatedBy,
        CreatedAt = p.CreatedAt,
        UpdatedBy = p.UpdatedBy,
        UpdatedAt = p.UpdatedAt
    })
    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<IEnumerable<ProductDto>> SearchByProductAsync(string product)
        {
            var details = await _context.Products
                .Where(c => c.ProductName.Contains(product))
                .Select(c => new ProductDto
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName
                })
                .ToListAsync();
            return details;
        }

        // --- Add a new Product ---
        public async Task<ProductDto> AddProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                InvoiceId = productDto.InvoiceId,
                CustomerId = productDto.CustomerId,
                ProductName = productDto.ProductName,
                
                Category = productDto.Category,
                Quantity = productDto.Quantity,
                Price = productDto.Price,
                Discount = productDto.Discount,
                TotalPrice = productDto.TotalPrice
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.ProductId = product.ProductId;

            return productDto;
        }

        // --- Update an existing Product ---
        public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            product.ProductName = productDto.ProductName;
            
            product.Category = productDto.Category;
            product.Quantity = productDto.Quantity;
            product.Price = productDto.Price;
            product.Discount = productDto.Discount;
            product.TotalPrice = productDto.TotalPrice;

            await _context.SaveChangesAsync();

            return productDto;
        }

        // --- Delete a Product ---
        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

