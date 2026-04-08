using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using MedUnit.Domain.Entities;
using MedUnit.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace RandevuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public ContactController(AppDbContext db, IEmailService emailService, IConfiguration config)
        {
            _db = db;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ContactMessageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new ContactMessage
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone.Replace(" ", "").Replace("-", ""),
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _db.ContactMessages.Add(entity);
            await _db.SaveChangesAsync();
            Console.WriteLine($"==> CONTACT REQUEST GELDI: {dto.Email}");
            try
            {
                string konu = $"Yeni İletişim Mesajı: {entity.Name} {entity.LastName}";
                string icerik = $"""
                    <h3>Yeni İletişim Mesajı</h3>
                    <p><b>Ad Soyad:</b> {entity.Name} {entity.LastName}</p>
                    <p><b>Email:</b> {entity.Email}</p>
                    <p><b>Telefon:</b> {entity.Phone}</p>
                    <p><b>Mesaj:</b><br/>{entity.Message}</p>
                    <hr/>
                    <small>Gönderim zamanı: {entity.CreatedAt:dd.MM.yyyy HH:mm}</small>
                """;

                var alici = _config["Email:AliciAdresi"]
         ?? _config["Email:ToAddress"]
         ?? "yunuskobal1233@gmail.com";

                Console.WriteLine($"==> ALICI: {alici}");

                await _emailService.GonderAsync(alici, konu, icerik);
                Console.WriteLine("==> MAIL GONDERILDI");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Hatası] {ex.Message}");
                Console.WriteLine($"==> MAIL HATASI: {ex.Message}");
                Console.WriteLine($"==> STACK: {ex.StackTrace}");
                throw; // geçici olarak ekleyin
            }

            return Ok(new { message = "Mesajınız alındı." });
        }
    }
}