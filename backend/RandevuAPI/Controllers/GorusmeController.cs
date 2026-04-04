using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RandevuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GorusmeController : ControllerBase
    {
        [HttpGet("baglanti-bilgisi")]
        public IActionResult BaglantiBilgisi()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ad = User.FindFirstValue(ClaimTypes.Name) ?? "Kullanıcı";

            return Ok(new
            {
                HubUrl = "/hubs/gorusme",
                KullaniciId = kullaniciId,
                Ad = ad
            });
        }
    }
}
