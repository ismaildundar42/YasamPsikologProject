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
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
                return NotFound(new { message = "Danışan bulunamadı." });

            return Ok(client);
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
                
                // User bilgisiyle birlikte dön
                createdClient.User = createdUser;
                
                return CreatedAtAction(nameof(GetById), new { id = createdClient.Id }, createdClient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            if (id != client.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var updatedClient = await _clientService.UpdateAsync(client);
                return Ok(updatedClient);
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
