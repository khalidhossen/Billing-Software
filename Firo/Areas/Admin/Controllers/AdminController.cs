using Firo.Application.ViewModels;
using Firo.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin"), Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILookUpRepository _lookUpRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPayDueRepository _payDueRepository;
        public AdminController(
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,
            ICompanyProfileRepository companyProfileRepository,
            IBranchRepository branchRepository,
            ILookUpRepository lookUpRepository,
            IProductRepository productRepository,
            IPayDueRepository payDueRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _companyProfileRepository = companyProfileRepository;
            _branchRepository = branchRepository;
            _lookUpRepository = lookUpRepository;
            _productRepository = productRepository;
            _payDueRepository = payDueRepository;

        }

        private Guid currUserGuid => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid) ? userGuid : Guid.Empty;
        [Route("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var model = new DashBoardVM
            {
                TotalCustomers = await _customerRepository.GetTotalCustomersAsync(),
                TodaysNewCustomers = await _customerRepository.GetTodaysCustomersAsync(),
                TotalSales = await _invoiceRepository.GetTotalSalesAmountAsync(),
                TodaysSales = await _invoiceRepository.GetTodaysSalesAmountAsync(),
                TotalDue = await _invoiceRepository.GetTotalDueAmountAsync(),
                TodaysDue = await _invoiceRepository.GetTodaysDueAmountAsync(),

                DailyLabels = Enumerable.Range(0, 7)
                 .Select(i => DateTime.Today.AddDays(-6 + i).ToString("ddd")) // ["Sat", "Sun", ...]
                 .ToList(),
                DailySales = await _invoiceRepository.GetDailySalesLast7DaysAsync(),
                DailyCustomers = await _customerRepository.GetDailyCustomersLast7DaysAsync(),

                MonthlyLabels = Enumerable.Range(0, 6)
                  .Select(i => DateTime.Today.AddMonths(-5 + i).ToString("MMM"))
                  .ToList(),
                MonthlySales = await _invoiceRepository.GetMonthlySalesLast6MonthsAsync(),
                MonthlyCustomers = await _customerRepository.GetMonthlyCustomersLast6MonthsAsync(),
            };

            return View(model);
        }
    }
}
