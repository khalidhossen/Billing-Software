using Firo.Application.Models;
using Firo.Common.Services;
using Firo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Firo.Common.Helper;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Route("ProfileSetup")]
    public class CompanySetupController : Controller
    {
        private readonly ICompanyProfileRepository _companyProfileRepository;
        public readonly FileUploadService _fileUploadService;

        public CompanySetupController(ICompanyProfileRepository companyProfileRepository, FileUploadService fileUploadService)
        {
            _companyProfileRepository = companyProfileRepository;
            _fileUploadService = fileUploadService;
        }

        private Guid currUserGuid => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid) ? userGuid : Guid.Empty;

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var companyProfile = await _companyProfileRepository.GetAllCompanyProfileAsync();    
            return View(companyProfile);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            CompanyProfileDto model = new CompanyProfileDto();
            return View(model);
        }

        [HttpPost("CreateCompany")]
        public async Task<IActionResult> Create(CompanyProfileDto companyProfileDto)
        {
            companyProfileDto.CreatedBy = currUserGuid;

            if (companyProfileDto.Logo != null && companyProfileDto.Logo.Length > 0)
            {
                var filename = _fileUploadService.UploadFileAsync(companyProfileDto.Logo, UploadFilePath.CompanyLogo);
                companyProfileDto.LogoString = filename.Result;
            }

            try
            {
                await _companyProfileRepository.AddCompanyProfileAsync(companyProfileDto);

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
            var companyProfile = await _companyProfileRepository.GetByIdAsync(id);
            if (companyProfile == null)
            {
                return NotFound();
            }

            return View("Create", companyProfile);
        }

        [HttpPost, Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(CompanyProfileDto companyProfileDto)
        {
            companyProfileDto.UpdatedBy = currUserGuid;

            if (companyProfileDto.Logo != null && companyProfileDto.Logo.Length > 0)
            {
                var filename = _fileUploadService.UploadFileAsync(companyProfileDto.Logo, UploadFilePath.CompanyLogo);
                companyProfileDto.LogoString = filename.Result;
            }
            else
            {
                companyProfileDto.LogoString = companyProfileDto.LogoPath;
            }

                try
                {
                    var updatedCompanyProfile = await _companyProfileRepository.UpdateCompanyProfileAsync(companyProfileDto);

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
            var companyProfile = await _companyProfileRepository.GetByIdAsync(id);

            if (companyProfile == null)
            {
                return NotFound();
            }

            bool result = await _companyProfileRepository.DeleteCompanyProfileAsync(id);

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
