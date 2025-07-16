using Firo.Application.Models;
using Firo.Common.Services;
using Firo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Firo.Common.Helper;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Firo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Route("Branch")]
    public class BranchController : Controller
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository;

        public BranchController(IBranchRepository branchRepository, ICompanyProfileRepository companyProfileRepository)
        {
            _branchRepository = branchRepository;
            _companyProfileRepository = companyProfileRepository;
        }

        private Guid currUserGuid => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid) ? userGuid : Guid.Empty;

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var branch = await _branchRepository.GetAllBranchAsync();

            return View(branch);
        }

        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            BranchDto model = new BranchDto();

            var companyProfile = await _companyProfileRepository.GetAllCompanyProfileAsync();

            if (companyProfile == null || !companyProfile.Any())
            {
                ViewBag.Companys = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No Company Found!", Value = "" }
                };
            }
            else
            {
                ViewBag.Companys = companyProfile.Select(x => new SelectListItem
                {
                    Text = x.CompanyName,
                    Value = x.CompanyProfileId.ToString()
                }).ToList();
            }


            return View(model);
        }

        [HttpPost("CreateBranch")]
        public async Task<IActionResult> Create(BranchDto branchDto)
        {
            branchDto.CreatedBy = currUserGuid;

            try
            {
                await _branchRepository.AddBranchAsync(branchDto);

                return Json(new { success = true, msg = "Data Saved!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Not Saved!", sdf = "asdf" });
            }
        }

        [Route("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var branchProfile = await _branchRepository.GetByIdAsync(id);
            if (branchProfile == null)
            {
                return NotFound();
            }

            var companyProfile = await _companyProfileRepository.GetAllCompanyProfileAsync();

            if (companyProfile == null || !companyProfile.Any())
            {
                ViewBag.Companys = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No Company Found!", Value = "" }
                };
            }
            else
            {
                ViewBag.Companys = companyProfile.Select(x => new SelectListItem
                {
                    Text = x.CompanyName,
                    Value = x.CompanyProfileId.ToString()
                }).ToList();
            }

            return View("Create", branchProfile);
        }

        [HttpPost, Route("UpdateBranch")]
        public async Task<IActionResult> UpdateBranch(BranchDto branchDto)
        {
            branchDto.UpdatedBy = currUserGuid;

            try
            {
                var updatedBranch = await _branchRepository.UpdateBranchAsync(branchDto);

                return Json(new { success = true, msg = "Data Saved!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Not Saved!", sdf = "asdf" });
            }

        }

        [Route("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var branchProfile = await _branchRepository.GetByIdAsync(id);

            if (branchProfile == null)
            {
                return NotFound();
            }

            bool result = await _branchRepository.DeleteBranchAsync(id);

            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(NotFound));
            }

        }

    }
}
