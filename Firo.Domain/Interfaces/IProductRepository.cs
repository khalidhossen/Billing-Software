using Firo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetByIdAsync(Guid productId);
        Task<ProductDto> AddProductAsync(ProductDto productDto);
        Task<ProductDto> UpdateProductAsync(ProductDto productDto);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<IEnumerable<ProductDto>> SearchByProductAsync(string product);
    }
}
