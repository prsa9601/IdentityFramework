using Infrastructure.Persistent.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;
using Infrastucture;
using Microsoft.EntityFrameworkCore;
using Domain.Login;
using Infrastructure.Repositories;
using Domain.UserAgg;
using Infrastucture.Tools;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using NPOI.SS.UserModel;
using Domain;
using Domain.UserAppAgg;
using Microsoft.AspNetCore.Authorization;
using Domain.RoleAgg.Repository;
using Microsoft.AspNetCore.Http;
using Domain.UserAgg.Repository;
using Domain.RoleAgg;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Pages.Account;
using Domain.identityUser;
using ServiceStack;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace cleanshop1.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {


        // private readonly IGoogleRecaptcha _Recaptcha;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserApp> _userManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly MyDBContext _context;
        private IRoleRepository _roleRepositories;
        private readonly IEmailSend _emailSend;
        private readonly IViewRenderService _viewRenderService;
        private IUserRepository _userRepository;
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        public UserController(IDistributedCache cache, IConnectionMultiplexer redis, ILogger<HomeController> logger, RoleManager<Role> roleManager, IRoleRepository roleRepositories, IUserRepository userRepositories, UserManager<UserApp> userManager, SignInManager<UserApp> signInManager, MyDBContext context)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleRepositories = roleRepositories;
            _userRepository = userRepositories;
            _cache = cache;
            _redis = redis;
        }

    



        [HttpPost("login")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "captcha_*").AsQueryable().Select(p => p).Last();

            var result6 = redisKeys.ToString();


            if (result6 == null || result6 != $"captcha_{model.Captcha.ToString()}")
            {
                ModelState.AddModelError("Captcha", "مجموع اشتباه است");
            }
           

            else
            {
                if (!ModelState.IsValid) return StatusCode(400, "اطلاعات ثبت شده اشتباه است");
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد");
                        return BadRequest(ModelState);
                    }
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {                      
                        var result1 = await _userRepository.Login(model.UserName,model.Password);
                        var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model.UserName));
                        _cache.Set("UserName_" + model.UserName.ToString(), content, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
                        return Ok(result1);

                        if (result1 == "هیچ کاربری در دیتابیس ذخیره نشده!")
                        {
                           return BadRequest(result1);
                        }
                    }
                    else
                    {
                            ModelState.AddModelError(string.Empty,result.ToString());
                    }
                }
            }
            return BadRequest(ModelState);
        }



        

        [HttpPost("LogOut")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogOut()
        {
            var result = _signInManager.SignOutAsync();
            if (result.IsCompleted)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(400," خطای سیستمی... لطفا مجددا اقدام نمایید!!!");
            }
        }
        [HttpPost("FirstRegister")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FirstRegister([FromForm] Domain.UserAgg.Register RegisterModel, int Captcha)
        {
         
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "captcha_*").AsQueryable().Select(p => p).Last();

            var result6 = redisKeys.ToString();

            if (result6 == null || result6 != $"captcha_{Captcha.ToString()}")
            {
                ModelState.AddModelError("Captcha", "مجموع اشتباه است"); 
                   return BadRequest(ModelState);
            }
            else
            {
                if (RegisterModel.Password.Trim() == RegisterModel.Password_Again.Trim())
                {
                    var user = new UserApp
                    {
                        UserName = RegisterModel.UserName,
                        Email = RegisterModel.Email,
                        PhoneNumber = RegisterModel.PhoneUser,
                        FirstName = RegisterModel.Name,
                        Role = "Admin",
                        Password = RegisterModel.Password,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    if (_userRepository.Exist())
                    {
                        var result1 = await _userManager.CreateAsync(user, RegisterModel.Password);
                        var result = await _userManager.AddToRoleAsync(user, "Admin");
                        if (result.Succeeded)
                        {
                            return StatusCode(200);
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return BadRequest(ModelState);
                    }
                    else
                        return StatusCode(400,"یک کاربر Admin ذخیره شد.");
                }
            }
                return BadRequest(ModelState);
        }
        [HttpPost("Register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromForm] Domain.UserAgg.Register RegisterModel, int Captcha)   
        {
        
            var redisKeys = _redis.GetServer("localhost", 9191).Keys(pattern: "captcha_*").AsQueryable().Select(p => p).Last();

            var result6 = redisKeys.ToString();



            if (result6 == null || result6 != $"captcha_{Captcha.ToString()}")
            {
                ModelState.AddModelError("Captcha", "مجموع اشتباه است");
            }
            else
            {
                if (RegisterModel.Password.Trim() == RegisterModel.Password_Again.Trim())
                {
                    var user = new UserApp
                    {
                        UserName = RegisterModel.UserName,
                        Email = RegisterModel.Email,
                        PhoneNumber = RegisterModel.PhoneUser,
                        FirstName = RegisterModel.Name,
                        Role = "Guest",
                        Password = RegisterModel.Password,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    var result1 = await _userManager.CreateAsync(user, RegisterModel.Password);
                    var result = await _userManager.AddToRoleAsync(user, "Guest");
                    if (result.Succeeded)
                    {
                        return StatusCode(201);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            return StatusCode(400, "خطای سیستمی ... لطفا مجددا اقدام نمایید !!!");
        }
        ////////////////////////////////////////    register    //////////////////////////////////
        #region register










        //[HttpPost("Register")]
        //public async Task<IActionResult> Register([FromForm]User u, int captcha)
        //{
        //    if (HttpContext.Session.GetString("Captcha").ToString() != captcha.ToString())
        //    {
        //        ModelState.AddModelError("Captcha", "مجموع اشتباه است");
        //    }
        //    else
        //    {
        //        if (u.Password.Trim() == u.Password_Again.Trim())
        //        {
        //            var user = new userapp
        //            {
        //                UserName = u.UserName,
        //                Email = u.Email,
        //                PhoneNumber = u.PhoneUser,
        //                firstname = u.Name,
        //                role = "Guest"
        //            };
        //                var result = await _userManager.CreateAsync(user, u.Password);
        //               // var result3 = await _signInManager.PasswordSignInAsync(u.UserName, u.Password, true, false);
        //                if (result.Succeeded) 
        //                {
        //                    return StatusCode(201);
        //                }
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //            return BadRequest(ModelState);
        //                //var usermodel = new User
        //                //{
        //                //    Email= u.Email,
        //                //    PhoneUser = u.PhoneUser,
        //                //    Password=u.Password,
        //                //    Password_Again = u.Password_Again,
        //                //    UserName = u.UserName,
        //                //    DeleteStatus = false,

        //                //};
        //        }
        //    }
        //    return StatusCode(400);
        //}
        #endregion

   
    }
}
