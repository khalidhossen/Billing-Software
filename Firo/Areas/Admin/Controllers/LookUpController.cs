using Firo.Application.Models;
using Firo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Route("LookUp")]
    public class LookUpController : Controller
    {
        private readonly ILookUpRepository _lookUpRepository;

        public LookUpController(ILookUpRepository lookUpRepository)
        {
            _lookUpRepository = lookUpRepository;
        }

        private Guid currUserGuid =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid) ? userGuid : Guid.Empty;

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var lookUps = await _lookUpRepository.GetAllLookUpAsync();
            return View(lookUps);
        }

        [Route("CreateLookUp")]
        public async Task<IActionResult> CreateLookUp()
        {
            
                ViewBag.DataList = await _lookUpRepository.GetAllLookUpAsync();
                LookUpDto model = new LookUpDto();
                model.IsActive = true;
                return View(model);
            
        }

        [HttpPost("CreateLookUp")]
        public async Task<IActionResult> CreateLookUp(LookUpDto lookUpDto)
        {
            try
            {
                lookUpDto.CreatedBy = currUserGuid;
                await _lookUpRepository.AddLookUpAsync(lookUpDto);
                return Json(new { success = true, msg = "LookUp saved successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "Failed to save LookUp!" });
            }
        }

        [Route("Edit/{LookUpId}")]
        public async Task<IActionResult> Edit(Guid LookUpId)
        {
            var lookUp = await _lookUpRepository.GetByIdAsync(LookUpId);
            if (lookUp == null) return NotFound();

            return View("Create", lookUp);
        }

        [HttpPost("UpdateLookUp")]
        public async Task<IActionResult> UpdateLookUp(LookUpDto lookUpDto)
        {
            try
            {
                lookUpDto.UpdatedBy = currUserGuid;
                var updated = await _lookUpRepository.UpdateLookUpAsync(lookUpDto);
                return Json(new { success = true, msg = "LookUp updated successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "Failed to update LookUp!" });
            }
        }

        [HttpPost("Delete/{LookUpId}")]
        public async Task<IActionResult> Delete(Guid LookUpId)
        {
            bool deleted = await _lookUpRepository.DeleteLookUpAsync(LookUpId);
            if (deleted == false)
            {
                return Json(new { success = false, msg = "LookUp not found!" });
            }
            else
            {
                return Json(new
                {
                    success = deleted,
                    msg = deleted ? "LookUp deleted successfully!" : "Failed to delete LookUp!"
                });
            }
                
        }

    }
}

