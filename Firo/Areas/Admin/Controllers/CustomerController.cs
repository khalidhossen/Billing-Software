using Firo.Application.Models;
using Firo.Common.Services;
using Firo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository;
        private readonly ILookUpRepository _lookUpRepository;

        public CustomerController(
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            ICompanyProfileRepository companyProfileRepository,
            ILookUpRepository lookUpRepository)
        {
            _customerRepository = customerRepository;
            _branchRepository = branchRepository;
            _companyProfileRepository = companyProfileRepository;
            _lookUpRepository = lookUpRepository;
        }

        private Guid currUserGuid =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid)
                ? userGuid
                : Guid.Empty;

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List(int page = 1, int pageSize = 4)
        {
            var paginatedData = await _customerRepository.GetCustomersPagedAsync(page, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("/Areas/Admin/Views/PartialViews/_CustomerTablePartial.cshtml", paginatedData);
            }

            return View(paginatedData);
        }


        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var customerTypes = await _lookUpRepository.GetByDataKeyAsync("CustomerType");
            ViewBag.CustomerTypes = customerTypes;

            var model = new CustomerDto();
            return View(model);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            customerDto.CreatedBy = currUserGuid;
            customerDto.UpdatedBy = currUserGuid;

            try
            {
                await _customerRepository.AddCustomerAsync(customerDto);
                return Json(new { success = true, msg = "Customer saved successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "Error saving customer." });
            }
        }

        [Route("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();

            await PopulateDropdownsAsync(customer.CompanyProfileId);
            return View("Create", customer); // Reuse Create.cshtml for editing
        }

        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(CustomerDto customerDto)
        {
            customerDto.UpdatedBy = currUserGuid;

            try
            {
                await _customerRepository.UpdateCustomerAsync(customerDto);
                return Json(new { success = true, msg = "Customer updated successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "Error updating customer." });
            }
        }

        [Route("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _customerRepository.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(List));
        }

        private async Task PopulateDropdownsAsync(Guid selectedCompanyId)
        {
            var companies = await _companyProfileRepository.GetAllCompanyProfileAsync();
            ViewBag.Companys = companies.Select(c => new SelectListItem
            {
                Text = c.CompanyName,
                Value = c.CompanyProfileId.ToString(),
                Selected = c.CompanyProfileId == selectedCompanyId
            }).ToList();

            var branches = await _branchRepository.GetAllBranchAsync();
            ViewBag.Branches = branches.Select(b => new SelectListItem
            {
                Text = b.BranchName,
                Value = b.BranchId.ToString()
            }).ToList();
        }

        [HttpGet("GetCustomerByPhone")]
        public JsonResult GetCustomerByPhone(string searchPhone)
        {
            if (string.IsNullOrEmpty(searchPhone) && searchPhone.Length < 4)
                return Json(new List<CustomerDto>());

            var customers = _customerRepository.GetCustomerListByPhoneNumber(searchPhone);

            if(customers == null)
                return Json(new List<CustomerDto>());
            else
                return Json(customers);
        }
    }
}
