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
            try
            {
                var psychologists = await _psychologistService.GetAllActiveAsync();
                
                // Entity'leri DTO'ya dönüştür
                var dtos = psychologists.Select(p => new
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    User = p.User == null ? null : new
                    {
                        Id = p.User.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Email = p.User.Email,
                        PhoneNumber = p.User.PhoneNumber,
                        Role = p.User.Role.ToString(),
                        IsActive = p.User.IsActive,
                        CreatedAt = p.User.CreatedAt
                    },
                    Biography = p.Biography,
                    CalendarColor = p.CalendarColor,
                    IsActive = p.IsActive
                }).ToList();
                
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Psikologlar listelenirken hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var psychologist = await _psychologistService.GetByIdAsync(id);
                if (psychologist == null)
                    return NotFound(new { message = "Psikolog bulunamadı." });

                // Entity'yi DTO'ya dönüştür
                var dto = new
                {
                    Id = psychologist.Id,
                    UserId = psychologist.UserId,
                    User = psychologist.User == null ? null : new
                    {
                        Id = psychologist.User.Id,
                        FirstName = psychologist.User.FirstName,
                        LastName = psychologist.User.LastName,
                        Email = psychologist.User.Email,
                        PhoneNumber = psychologist.User.PhoneNumber,
                        Role = psychologist.User.Role.ToString(),
                        IsActive = psychologist.User.IsActive,
                        CreatedAt = psychologist.User.CreatedAt
                    },
                    Biography = psychologist.Biography,
                    CalendarColor = psychologist.CalendarColor,
                    IsActive = psychologist.IsActive
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Psikolog getirilirken hata oluştu: {ex.Message}" });
            }
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Geçersiz veri.", errors = errors });
            }

            try
            {
                // Önce User oluştur
                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = "Temp123!", // Geçici şifre - Test için hashlenmeden
                    Role = UserRole.Psychologist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userService.CreateAsync(user);

                // Sonra Psychologist oluştur
                var psychologist = new Psychologist
                {
                    UserId = createdUser.Id,
                    CalendarColor = dto.CalendarColor ?? "#4CAF50",
                    IsActive = dto.IsActive
                };

                var createdPsychologist = await _psychologistService.CreateAsync(psychologist);
                
                // DTO formatında dön
                var responseDto = new
                {
                    Id = createdPsychologist.Id,
                    UserId = createdUser.Id,
                    User = new
                    {
                        Id = createdUser.Id,
                        FirstName = createdUser.FirstName,
                        LastName = createdUser.LastName,
                        Email = createdUser.Email,
                        PhoneNumber = createdUser.PhoneNumber,
                        Role = createdUser.Role.ToString(),
                        IsActive = createdUser.IsActive,
                        CreatedAt = createdUser.CreatedAt
                    },
                    Biography = createdPsychologist.Biography,
                    CalendarColor = createdPsychologist.CalendarColor,
                    IsActive = createdPsychologist.IsActive
                };
                
                return CreatedAtAction(nameof(GetById), new { id = createdPsychologist.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePsychologistDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Geçersiz veri.", errors = errors });
            }

            try
            {
                // Mevcut psikolog kaydını getir
                var existing = await _psychologistService.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Psikolog bulunamadı." });

                // User bilgilerini güncelle
                var user = existing.User;
                if (user != null)
                {
                    user.FirstName = dto.FirstName;
                    user.LastName = dto.LastName;
                    user.Email = dto.Email;
                    user.PhoneNumber = dto.PhoneNumber;
                    user.UpdatedAt = DateTime.UtcNow;
                    
                    await _userService.UpdateAsync(user);
                }

                // Psychologist bilgilerini güncelle
                existing.CalendarColor = dto.CalendarColor ?? "#4CAF50";
                existing.IsActive = dto.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;

                var updatedPsychologist = await _psychologistService.UpdateAsync(existing);
                
                // DTO formatında dön
                var responseDto = new
                {
                    Id = updatedPsychologist.Id,
                    UserId = updatedPsychologist.UserId,
                    User = user == null ? null : new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString(),
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt
                    },
                    Biography = updatedPsychologist.Biography,
                    CalendarColor = updatedPsychologist.CalendarColor,
                    IsActive = updatedPsychologist.IsActive
                };
                
                return Ok(responseDto);
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
