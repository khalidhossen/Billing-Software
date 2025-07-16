using Firo.Domain.Interfaces;
using Firo.Infrastructure.Repositories;

namespace Firo.Web.Service
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<ILookUpRepository, LookUpRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPayDueRepository, PayDueRepository>();
        }
    }
}
