using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace cleanshop1.Controllers
{
    public class JwtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            var key = Encoding.ASCII.GetBytes("Jwt Token");
            var TokenHandler = new JwtSecurityTokenHandler();
            return View();
        }
    }
}
