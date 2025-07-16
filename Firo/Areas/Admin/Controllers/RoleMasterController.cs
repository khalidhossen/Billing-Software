using Firo.Common.Helper;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Role.Admin)]
    [Route("Role"), Area("Admin")]
    public class RoleMasterController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyProfileRepository _companyProfileRepository;

        public RoleMasterController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager
            , ICompanyProfileRepository companyProfileRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyProfileRepository = companyProfileRepository;
        }
        private async Task<List<object>> GetUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    foreach (var role in roles)
                    {
                        usersWithRoles.Add(new { User = user, Role = role });
                    }
                }
                else
                {
                    usersWithRoles.Add(new { User = user, Role = "-" });
                }
            }

            return usersWithRoles;
        }
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var usersWithRoles = await GetUsersWithRolesAsync();

            return Json(usersWithRoles);
        }
        [HttpPost, Route("AssignRole")]
        public async Task<IActionResult> AssignRole(string UserName, string Role)
        {
            // Validate input
            if (string.IsNullOrEmpty(UserName))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            if (string.IsNullOrEmpty(Role))
            {
                return BadRequest(new { message = "Role is required." });
            }

            // Find user by ID
            var user = await _userManager.FindByIdAsync(UserName);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(Role);
            if (!roleExists)
            {
                return NotFound(new { message = $"Role '{Role}' does not exist." });
            }

            // Check if the user is already in the role
            var isInRole = await _userManager.IsInRoleAsync(user, Role);
            if (isInRole)
            {
                return BadRequest(new { message = $"User '{user.UserName}' is already assigned to the role '{Role}'." });
            }

            // Assign the role to the user
            var result = await _userManager.AddToRoleAsync(user, Role);
            if (!result.Succeeded)
            {
                return StatusCode(500, new
                {
                    message = "Failed to assign role. Please try again later.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new { message = $"Role '{Role}' assigned to user '{user.UserName}' successfully!" });
        }

        [HttpPost, Route("RemoveRole")]
        public async Task<IActionResult> RemoveRole(string UserId, string Role)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Role))
            {
                return BadRequest(new { message = "User ID or Role is missing." });
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, Role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role removed successfully!" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while removing the role." });
        }

        [Route("Management")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.Companys = await _companyProfileRepository.GetAllCompanyProfileAsync();
            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();


            var usersWithRoles = await GetUsersWithRolesAsync();


            return View(usersWithRoles);
        }
    }
}
