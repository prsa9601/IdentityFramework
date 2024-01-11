using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using Domain.UserAgg;

namespace cleanshop1.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : Controller
    {

        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        public CaptchaController(IDistributedCache cache, IConnectionMultiplexer redis) 
        {
            _cache = cache;
            _redis = redis;
        }
        [HttpGet]
        [Route("CaptchaImage")]
        public ActionResult CaptchaImage(bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            var e = a + b;
      
            var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(e));

             _cache.Set("captcha_" + e.ToString(), content, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(30) });

            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((System.Drawing.Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }

                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;

        }
    }
}
