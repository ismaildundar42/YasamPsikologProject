using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(user);
        }

        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone)
        {
            var user = await _userService.GetByPhoneAsync(phone);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            try
            {
                var createdUser = await _userService.CreateAsync(user);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var updatedUser = await _userService.UpdateAsync(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("email-exists")]
        public async Task<IActionResult> EmailExists([FromQuery] string email, [FromQuery] int? excludeUserId = null)
        {
            var exists = await _userService.EmailExistsAsync(email, excludeUserId);
            return Ok(new { exists });
        }

        [HttpGet("phone-exists")]
        public async Task<IActionResult> PhoneExists([FromQuery] string phone, [FromQuery] int? excludeUserId = null)
        {
            var exists = await _userService.PhoneExistsAsync(phone, excludeUserId);
            return Ok(new { exists });
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Geçersiz veri.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "Kullanıcı bulunamadı." });

                // Mevcut şifreyi kontrol et - Hem BCrypt hem de düz metin desteği
                bool isCurrentPasswordValid = false;
                
                try
                {
                    if (user.PasswordHash.StartsWith("$2"))
                    {
                        // BCrypt hash formatı
                        isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
                    }
                    else
                    {
                        // Düz metin şifre
                        isCurrentPasswordValid = user.PasswordHash == dto.CurrentPassword;
                    }
                }
                catch
                {
                    // BCrypt hatası olursa düz metin kontrolü yap
                    isCurrentPasswordValid = user.PasswordHash == dto.CurrentPassword;
                }

                if (!isCurrentPasswordValid)
                {
                    return BadRequest(new { message = "Mevcut şifre yanlış." });
                }

                // Yeni şifreyi kaydet
                user.PasswordHash = dto.NewPassword; // Geçici olarak hashlemeden direkt string
                // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                
                await _userService.UpdateAsync(user);
                
                return Ok(new { message = "Şifre başarıyla değiştirildi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
