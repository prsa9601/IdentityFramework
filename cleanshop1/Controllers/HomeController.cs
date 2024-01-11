//using DNTCaptcha.Core;
using Infrastructure.Persistent.Ef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Reflection;
using Microsoft.AspNetCore.Session;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using SkiaSharp;
using NPOI.SS.Formula.Functions;
//using BotDetect;
using System.Text;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Drawing.Imaging;
using Domain.ProductAgg;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using BotDetect.Infrastructure.Common;
using Microsoft.VisualBasic;
using Domain.Login;

namespace cleanshop1.Controllers
{
    public class HomeController : Controller
    {
       // private readonly UserManager<userapp> _userManager;
        //private readonly SignInManager<userapp> _signInManager;
        private readonly ILogger<HomeController> _logger;
        //private IDNTCaptchaValidatorService _validatorService;
        //private DNTCaptchaOptions _captchaOptions;
        //, IDNTCaptchaValidatorService validatorService
        //, IOptions<DNTCaptchaOptions> options
        public HomeController(ILogger<HomeController> logger )
        {
          //  _signInManager = signInManager;
            //_userManager = userManager;
          //  _validatorService = validatorService;
            _logger = logger;
           // _captchaOptions = options == null ? throw new ArgumentException(nameof(options)) : options.Value;
        }

        public IActionResult Index()
        {
            TempData.Clear();
            TempData.Remove("UserName");
            return View();
        }
  
        public IActionResult Privacy()
        {
            return View();
        }

    }
}