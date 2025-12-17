using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;
using YasamPsikologProject.WebApi.DTOs;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PsychologistsController : ControllerBase
    {
        private readonly IPsychologistService _psychologistService;
        private readonly IUserService _userService;

        public PsychologistsController(
            IPsychologistService psychologistService,
            IUserService userService)
        {
            _psychologistService = psychologistService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var psychologists = await _psychologistService.GetAllActiveAsync();
            return Ok(psychologists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var psychologist = await _psychologistService.GetByIdAsync(id);
            if (psychologist == null)
                return NotFound(new { message = "Psikolog bulunamadı." });

            return Ok(psychologist);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var psychologist = await _psychologistService.GetByUserIdAsync(userId);
            if (psychologist == null)
                return NotFound(new { message = "Psikolog bulunamadı." });

            return Ok(psychologist);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePsychologistDto dto)
        {
            try
            {
                // Önce User oluştur
                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Temp123!"), // Geçici şifre
                    Role = UserRole.Psychologist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userService.CreateAsync(user);

                // Sonra Psychologist oluştur
                var psychologist = new Psychologist
                {
                    UserId = createdUser.Id,
                    LicenseNumber = dto.LicenseNumber,
                    Specialization = dto.Specialization,
                    CalendarColor = dto.CalendarColor ?? "#4CAF50",
                    IsActive = dto.IsActive
                };

                var createdPsychologist = await _psychologistService.CreateAsync(psychologist);
                
                // User bilgisiyle birlikte dön
                createdPsychologist.User = createdUser;
                
                return CreatedAtAction(nameof(GetById), new { id = createdPsychologist.Id }, createdPsychologist);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Psychologist psychologist)
        {
            if (id != psychologist.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var updatedPsychologist = await _psychologistService.UpdateAsync(psychologist);
                return Ok(updatedPsychologist);
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
                await _psychologistService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
