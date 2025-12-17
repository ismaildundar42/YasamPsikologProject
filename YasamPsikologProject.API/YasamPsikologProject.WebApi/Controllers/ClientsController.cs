using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;
using YasamPsikologProject.WebApi.DTOs;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IUserService _userService;

        public ClientsController(
            IClientService clientService,
            IUserService userService)
        {
            _clientService = clientService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _clientService.GetAllAsync();
                
                // Entity'leri DTO'ya dönüştür
                var dtos = clients.Select(c => new
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    User = c.User == null ? null : new
                    {
                        Id = c.User.Id,
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        Role = c.User.Role.ToString(),
                        IsActive = c.User.IsActive,
                        CreatedAt = c.User.CreatedAt
                    },
                    AssignedPsychologistId = c.AssignedPsychologistId,
                    AssignedPsychologist = c.AssignedPsychologist == null ? null : new
                    {
                        Id = c.AssignedPsychologist.Id,
                        UserId = c.AssignedPsychologist.UserId,
                        LicenseNumber = c.AssignedPsychologist.LicenseNumber,
                        Specialization = c.AssignedPsychologist.Specialization
                    },
                    Address = c.Address,
                    Notes = c.Notes,
                    KvkkConsent = c.KvkkConsentGiven,
                    KvkkConsentDate = c.KvkkConsentDate,
                    PreferredNotificationMethod = c.PreferredNotificationMethod?.ToString(),
                    IsActive = c.IsActive
                }).ToList();
                
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Danışanlar listelenirken hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var client = await _clientService.GetByIdAsync(id);
                if (client == null)
                    return NotFound(new { message = "Danışan bulunamadı." });

                // Entity'yi DTO'ya dönüştür
                var dto = new
                {
                    Id = client.Id,
                    UserId = client.UserId,
                    User = client.User == null ? null : new
                    {
                        Id = client.User.Id,
                        FirstName = client.User.FirstName,
                        LastName = client.User.LastName,
                        Email = client.User.Email,
                        PhoneNumber = client.User.PhoneNumber,
                        Role = client.User.Role.ToString(),
                        IsActive = client.User.IsActive,
                        CreatedAt = client.User.CreatedAt
                    },
                    AssignedPsychologistId = client.AssignedPsychologistId,
                    AssignedPsychologist = client.AssignedPsychologist == null ? null : new
                    {
                        Id = client.AssignedPsychologist.Id,
                        UserId = client.AssignedPsychologist.UserId,
                        LicenseNumber = client.AssignedPsychologist.LicenseNumber,
                        Specialization = client.AssignedPsychologist.Specialization
                    },
                    Address = client.Address,
                    Notes = client.Notes,
                    KvkkConsent = client.KvkkConsentGiven,
                    KvkkConsentDate = client.KvkkConsentDate,
                    PreferredNotificationMethod = client.PreferredNotificationMethod?.ToString(),
                    IsActive = client.IsActive
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Danışan getirilirken hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var client = await _clientService.GetByUserIdAsync(userId);
            if (client == null)
                return NotFound(new { message = "Danışan bulunamadı." });

            return Ok(client);
        }

        [HttpGet("psychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId)
        {
            var clients = await _clientService.GetByPsychologistAsync(psychologistId);
            return Ok(clients);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Temp123!"), // Geçici şifre
                    Role = UserRole.Client,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userService.CreateAsync(user);

                // Sonra Client oluştur
                var client = new Client
                {
                    UserId = createdUser.Id,
                    AssignedPsychologistId = dto.AssignedPsychologistId,
                    PreferredNotificationMethod = Enum.TryParse<NotificationType>(dto.PreferredNotificationMethod, out var notifType) 
                        ? notifType 
                        : NotificationType.Email,
                    KvkkConsentGiven = dto.KvkkConsent,
                    KvkkConsentDate = dto.KvkkConsent ? DateTime.UtcNow : null
                };

                var createdClient = await _clientService.CreateAsync(client);
                
                // DTO formatında dön
                var responseDto = new
                {
                    Id = createdClient.Id,
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
                    AssignedPsychologistId = createdClient.AssignedPsychologistId,
                    Address = createdClient.Address,
                    Notes = createdClient.Notes,
                    KvkkConsent = createdClient.KvkkConsentGiven,
                    KvkkConsentDate = createdClient.KvkkConsentDate,
                    PreferredNotificationMethod = createdClient.PreferredNotificationMethod?.ToString(),
                    IsActive = createdClient.IsActive
                };
                
                return CreatedAtAction(nameof(GetById), new { id = createdClient.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Geçersiz veri.", errors = errors });
            }

            try
            {
                // Mevcut client kaydını getir
                var existing = await _clientService.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Danışan bulunamadı." });

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

                // Client bilgilerini güncelle
                existing.AssignedPsychologistId = dto.AssignedPsychologistId;
                existing.PreferredNotificationMethod = Enum.TryParse<NotificationType>(dto.PreferredNotificationMethod, out var notifType)
                    ? notifType
                    : NotificationType.Email;
                existing.KvkkConsentGiven = dto.KvkkConsent;
                existing.UpdatedAt = DateTime.UtcNow;

                var updatedClient = await _clientService.UpdateAsync(existing);
                
                // DTO formatında dön
                var responseDto = new
                {
                    Id = updatedClient.Id,
                    UserId = updatedClient.UserId,
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
                    AssignedPsychologistId = updatedClient.AssignedPsychologistId,
                    Address = updatedClient.Address,
                    Notes = updatedClient.Notes,
                    KvkkConsent = updatedClient.KvkkConsentGiven,
                    KvkkConsentDate = updatedClient.KvkkConsentDate,
                    PreferredNotificationMethod = updatedClient.PreferredNotificationMethod?.ToString(),
                    IsActive = updatedClient.IsActive
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
                await _clientService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
