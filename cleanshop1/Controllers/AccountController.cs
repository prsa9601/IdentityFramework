using Domain.identityUser;
using Domain.UserAppAgg;
using Infrastucture.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.RoleAgg.Repository;

namespace cleanshop1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly EmailSend _emailSender;
        private readonly IViewRenderService _viewRenderService;
        private readonly IRoleRepository _roleRepository;

        public AccountController(UserManager<UserApp> userManager, EmailSend emailSender, IViewRenderService viewRenderService, SignInManager<UserApp> signInManager, IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
            _signInManager = signInManager;
            _roleRepository = roleRepository;
        }

        public IActionResult Register()
        {
            //ViewBag.IsSent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
       

            var result = await _userManager.CreateAsync(new UserApp()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone,
            }, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View();
                }
            }
            

            var user = await _userManager.FindByNameAsync(model.UserName);
            var mobileCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

            return RedirectToAction("ConfirmMobile", new { phone = user.PhoneNumber, token = mobileCode });

        }


        #region confirmmobileandEmail
        public IActionResult ConfirmMobile(string phone, string token)
        {
            if (phone == null || token == null) return BadRequest();
            ConfirmMobile vm = new ConfirmMobile()
            {
                Phone = phone,
                Code = token
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmMobile(ConfirmMobile model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError("", "کاربری یافت نشد");
                return View();
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.SmsCode);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده معتبر نمیباشد");
                return View(model);
            }

            user.PhoneNumberConfirmed = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.IsConfirmed = result.Succeeded ? true : false;
            return View();
        }
        #endregion


        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginWithPhoneVM()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginWithPhone model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد");
                return View(model);
            }

            var mobileCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

            return RedirectToAction("LoginConfirmation", new { phone = user.PhoneNumber, token = mobileCode });

            
        }

        public IActionResult LoginConfirmation(string phone, string token)
        {
            if (phone == null || token == null) return BadRequest();

            LoginConfirmation vm = new()
            {
                Phone = phone,
                Code = token
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> LoginConfirmation(LoginConfirmation model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری یافت نشد");
                return View(model);
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.SmsCode);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده معتبر نمیباشد");
                return View(model);
            }

            await _signInManager.SignInAsync(user, true);
            return Redirect("/");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            ViewBag.IsSent = false;
            return View();
        }

        #region forgotpass
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsSent = false;
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "تلاش برای ارسال ایمیل ناموفق بود");
                ViewBag.IsSent = false;
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ResetPassword", "Account", new { email = user.Email, token = token },
                Request.Scheme);
            string body = await _viewRenderService.RenderToStringAsync("_ResetPasswordEmail", callBackUrl);
            await _emailSender.SendEmailAsync(new EmailModel(user.Email, "بازیابی کلمه عبور", body));
            ViewBag.IsSent = true;
            return View();
        }

        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) return BadRequest();

            ResetPassword model = new ResetPassword()
            {
                Email = email,
                Token = token
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "تلاش برای بازیابی کلمه عبور ناموفق بود");
                return View(model);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }

            return RedirectToAction("Login");
        }

        #endregion

        #region External Login

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account",
                new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"));

            var model = new LoginWithPhoneVM()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error : {remoteError}");
                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error : {remoteError}");
                return View("Login", model);
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded)
                return Redirect(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    string userName = email.Split("@")[0].Replace(".", "");
                    user = new UserApp()
                    {
                        Email = email,
                        UserName = userName,
                        //NickName = userName,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = ""
                    };

                    await _userManager.CreateAsync(user);
                }

                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, false);
                return Redirect(returnUrl);
            }

            return View();
        }

        #endregion

        #region Remote Validations

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string userName)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.UserName == userName);
            if (!any)
                return Json(true);

            return Json("نام کاربری مورد نظر از قبل ثبت شده است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.Email == email);
            if (!any)
                return Json(true);

            return Json("ایمیل مورد نظر از قبل ثبت شده است");
        }

        #endregion
    }
}
