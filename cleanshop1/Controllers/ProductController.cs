using Domain.Permission;
using Domain.Permission.Repository;
using Domain.ProductAgg;
using Domain.ProductAgg.Repository;
using Domain.RoleAgg;
using Domain.RoleAgg.Repository;
//using Domain.UserAccessAgg;
using Domain.UserAgg;
using Domain.UserAgg.Repository;
using Domain.UserAppAgg;
using Domain.Utility;
using Infrastructure.Persistent.Ef;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace cleanshop1.Controllerس
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly MyDBContext _context;
        private IProductRepository _productRepository { get; }
        private IRoleRepository _roleRepository { get; }
        private IUserRepository _userRepository { get; }
        private IPermissionRepository _permissionRepository { get; }
        private readonly JWTSettings _JwtSetting;
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;

        private new readonly ClaimsPrincipal User;
        public ProductController(IDistributedCache cache, IConnectionMultiplexer redis, IUserRepository userRepositories, IOptions<JWTSettings> options, IPermissionRepository permissionRepositories, RoleManager<Role> roleManager, IRoleRepository roleRepositories, IProductRepository productRepository, MyDBContext context, UserManager<UserApp> userManager, SignInManager<UserApp> signInManager)
        {
            _cache = cache;
            _redis = redis;
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepositories;
            _roleRepository = roleRepositories;
            _roleManager = roleManager;
            _permissionRepository = permissionRepositories;
            _JwtSetting = options.Value; 
        }

        [HttpPost]
        [Route("CreateProduct")]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromForm] string name, [FromForm]string price)
        {
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "UserName_*").AsQueryable().Select(p => p).Last();

            string UserName = redisKeys.ToString().Replace("UserName_", "").Trim();
            UserApp user = await _userManager.FindByNameAsync(UserName);

            var product = new Product 
            { 
                Name = name,
                Price = price, 
                DeleteStatus = false,
                PhoneUser = user.PhoneNumber ,
                userId = user.Id,
                UserApp = user,
                UserName = UserName 
            };
            string result = await _productRepository.CreateProduct(product);
            if (result == "عملیات با موفقیت انجام شد.")
            {
                return StatusCode(201);
            }
            else
            {
                return BadRequest(ModelState);
            }
                    
            
        }
        [HttpPatch]
        [Authorize]
        [Route("UpdateProduct")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(int id, string ProductName,string price)
        {
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "UserName_*").AsQueryable().Select(p => p).Last();
            
            string UserName = redisKeys.ToString().Replace("UserName_", "").Trim();
            if (UserName == null) return StatusCode(400, "شما لاگین نکردید!");

            Role role = new Role();
            UserApp user = await _userManager.FindByNameAsync(UserName);

            role = _roleRepository.GetRoleByUserId(user.Role);

            List<Permission> permissions = new List<Permission>();
            permissions = _permissionRepository.GetPermission(role.Name);
            if (_productRepository.UserOwnsProduct(id, user.Id) == true)
            {
                string result = await _productRepository.UpdateProduct(id, user.UserName, ProductName, price);
                if (result == "عملیات با موفقیت انجام شد.")
                {
                    return StatusCode(200);
                }
                else return StatusCode(500);
            }
            else
            {
                var Permission = _permissionRepository.FilterPermission(role.Name, id);
                if (Permission != null && Permission.PermissionUpdate == true)
                {
                    string result = await _productRepository.UpdateProduct(id, user.UserName,ProductName, price);
                    if (result == "عملیات با موفقیت انجام شد.")
                    {
                        return StatusCode(200);
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
                else
                {
                    return BadRequest("شما دسترسی لازم را برای ویرایش محصول ندارید");
                }
            }
        }
        [HttpDelete]
        [Authorize]
        [Route("DeleteProduct")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> DeleteProduct(int id)
        {
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "UserName_*").AsQueryable().Select(p => p).Last();

            string UserName = redisKeys.ToString().Replace("UserName_", "").Trim();
            if (UserName == null) return StatusCode(400, "شما لاگین نکردید!");

            Role role = new Role();
            UserApp user = await _userManager.FindByNameAsync(UserName);
           
            role = _roleRepository.GetRoleByUserId(user.Role);

            List<Permission> permissions = new List<Permission>();  
            permissions = _permissionRepository.GetPermission(role.Name);
            if(_productRepository.UserOwnsProduct(id, user.Id) == true)
            {
                string DeleteResult = await _productRepository.DeleteProduct(id, user.UserName);
                return DeleteResult;
            }
            else
            {
                var Permission = _permissionRepository.FilterPermission(role.Name, id);
                if (Permission != null && Permission.PermissionDelete == true)
                {
                    string result = await _productRepository.DeleteProduct(id,user.UserName);
                    if (result == "عملیات با موفقیت انجام شد.")
                    {
                        return StatusCode(200);
                    }
                    return result;
                }
                else
                {
                    return BadRequest("شما دسترسی لازم را برای حذف محصول ندارید");
                }
            }
        } 
        [HttpGet]
        [Route("ReadProductByUser")]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> ReadProductByUser()
        {
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "UserName_*").AsQueryable().Select(p => p).Last();

            string UserName = redisKeys.ToString().Replace("UserName_", "").Trim();

            if (UserName == null) return StatusCode(400, "شما لاگین نکردید!");

            var result = _productRepository.ReadProductByUser(UserName);
            if (result != null)
            {
                return result;
            }
            else
            {
                return NotFound();
            }
         
            return null;
        }
        [HttpGet]
        [Route("FilterProductByUserName")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> FilterProductByUserName(string username)
        {
            if (ModelState.IsValid)
            {
                var result = _productRepository.FilterProductByUserName(username);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("ReadProductByUserNow")]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> ReadProductByUserNow()
        {
            if (ModelState.IsValid)
            {
                // string UserName = HttpContext.Session.GetString("username");
                var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "UserName_*").AsQueryable().Select(p => p).Last();

                string UserName = redisKeys.ToString().Replace("UserName_","").Trim();
                
                if (UserName == null) return StatusCode(400, "شما لاگین نکردید!");
                var result =   _productRepository.FilterProductByUserName(UserName);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("ReadProduct")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Product>>> ReadProduct()
        {
            if (ModelState.IsValid)
            {
                var result = _productRepository.ReadProduct();
                if (result!=null)
                {
                    return result;
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
