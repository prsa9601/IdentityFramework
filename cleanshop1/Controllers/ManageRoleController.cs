using Domain.RoleAgg;
using Domain.UserAgg.Repository;
using Domain.UserAgg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.ProductAgg;
using Infrastructure.Repositories;
using Domain.RoleAgg.Repository;
using Domain.Permission.Repository;
using Domain.UserAppAgg;
using Domain.ProductAgg.Repository;

namespace cleanshop1.Controllers
{
    [Authorize(Roles ="Admin")]
    [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
    public class ManageRoleController : Controller
    {
        
        private RoleManager<Role> _roleManager;
        private UserManager<UserApp> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public ManageRoleController(RoleManager<Role> roleManager, IPermissionRepository permissionRepositories , IRoleRepository roleRepositories , IUserRepository userRepositories ,UserManager<UserApp> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepository = userRepositories;
            _roleRepository = roleRepositories;
            _permissionRepository = permissionRepositories;
        }

     
        [HttpPost("AddRole")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>AddRole(string name)
        {
            var role = new Role { Name = name };
            var result = await _roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                return Ok();
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        [Route("DeleteRole")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRole(string RoleName)
        {
            var role = await _roleManager.FindByNameAsync(RoleName);
            if(role == null)
            {
                return NotFound();
            }
            await _roleManager.DeleteAsync(role);

            return StatusCode(200);
        }
        [HttpPost("CreateFirstRole")]
        [ProducesResponseType(201)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Exist()
        {
            bool n = _roleRepository.Exist();
            if (n == true)
            {
                var role = new Role
                {
                    Name = "Admin",
                    UserId = 1,
                    DeleteStatus = false,
                    roleId = 1,

                };
                var role1 = new Role
                {
                    Name = "Guest",
                    DeleteStatus = false,
                    roleId = 2,

                };
                var result3 = await _roleManager.CreateAsync(role);
                var result2 = await _roleManager.CreateAsync(role1);
                if (result3.Succeeded && result2.Succeeded)
                {
                    return StatusCode(200);
                }
                else
                {
                    foreach (var error in result3.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            else
                return StatusCode(400, "خطای سیستمی ... لطفا مجددا اقدام نمایید !!!");
        }


        [HttpPost]
        [Route("EditRole")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditRole(string RoleName, string NewRoleName)
        {
            var role = await _roleManager.FindByNameAsync(RoleName);
            if (role == null)
            {
                return StatusCode(404);
            }
            role.Name = NewRoleName;
            var result = await _roleManager.UpdateAsync(role);
            if(result.Succeeded)
            {
                return StatusCode(201);
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
               return BadRequest(ModelState);

        }
        [HttpPost("UseRole")]
        public async Task<IActionResult> UseRole(string username, string role)
        {
            UserApp user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles != null)
            {
                string roleName = userRoles.FirstOrDefault();
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }


            var result1 = await _userManager.AddToRoleAsync(user, role);
            if (result1.Succeeded)
            { 
               string result = await _userRepository.UseRole(username,role);
               if (result == "عملیات با موفقیت انجام شد.")
               {
                   return StatusCode(201);
               }
               else
               {
                   return BadRequest(ModelState);
               }
            }
            return BadRequest();
        }
    

    }
}
