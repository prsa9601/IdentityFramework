using Domain.RoleAgg.Repository;
using Domain.RoleAgg;
using Domain.UserAgg.Repository;
using Domain.UserAppAgg;
using Infrastructure.Persistent.Ef;
using Infrastucture.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Permission.Repository;

namespace cleanshop1.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyDBContext _context;
        private IRoleRepository _roleRepository;
        private readonly IEmailSend _emailSend;
        private readonly IViewRenderService _viewRenderService;
        private IUserRepository _userRepository;
        private IPermissionRepository _permission;

        public PermissionController(IPermissionRepository permissionRepository,  RoleManager<Role> roleManager, IRoleRepository roleRepositories, IUserRepository userRepositories, UserManager<UserApp> userManager, SignInManager<UserApp> signInManager, MyDBContext context)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleRepository = roleRepositories;
            _permission = permissionRepository;
        }
        [HttpPost("InsertPermission")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertPermission(int productid, string role, bool update, bool delete)
        {
            string result = await _permission.CreatePermission(productid, role, update, delete);
            if (result == "عملیات با موفقیت انجام شد.")
            {
               return StatusCode(201);
            }
            return BadRequest();
        }
        [HttpPatch("UpdatePermission")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePermission(int productId, string role, bool update, bool delete)
        {
            string result = await _permission.UpdatePermission(productId, role, update, delete);
            if (result == "عملیات با موفقیت انجام شد.")
            {
               return StatusCode(201);
            }
            return BadRequest();
        }
        [HttpDelete("DeletePermission")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePermission(int productId, string RoleName)
        {
            string result = _permission.DeletePermission(productId, RoleName);
            if (result == "عملیات با موفقیت انجام شد.")
            {
               return StatusCode(201);
            }
            return BadRequest();
        }

    }
}
