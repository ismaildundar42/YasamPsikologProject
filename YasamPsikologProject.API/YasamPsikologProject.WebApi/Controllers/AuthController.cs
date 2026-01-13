using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPsychologistService _psychologistService;
        private readonly IClientService _clientService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            IPsychologistService psychologistService,
            IClientService clientService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _psychologistService = psychologistService;
            _clientService = clientService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {request.Email}");
                
                // Find user by email
                var users = await _userService.GetAllAsync();
                var user = users.FirstOrDefault(u => 
                    u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && 
                    u.IsActive && 
                    !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning($"User not found or inactive: {request.Email}");
                    return Unauthorized(new { message = "Geçersiz email veya şifre." });
                }

                // Basit şifre kontrolü
                bool isPasswordValid = (user.PasswordHash == request.Password);
                
                _logger.LogInformation($"Email: {request.Email} | DB Password: [{user.PasswordHash}] | Input Password: [{request.Password}] | Match: {isPasswordValid}");

                if (!isPasswordValid)
                {
                    _logger.LogWarning($"Password validation failed for user: {user.Email}");
                    return Unauthorized(new { message = "Geçersiz email veya şifre." });
                }

                // Get additional IDs based on role
                int? psychologistId = null;
                int? clientId = null;

                if ((int)user.Role == 1) // Psychologist
                {
                    var psychologist = await _psychologistService.GetByUserIdAsync(user.Id);
                    psychologistId = psychologist?.Id;
                }
                else if ((int)user.Role == 2) // Client
                {
                    var client = await _clientService.GetByUserIdAsync(user.Id);
                    clientId = client?.Id;
                }

                var response = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    PsychologistId = psychologistId,
                    ClientId = clientId
                };

                _logger.LogInformation($"User {user.Email} logged in successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return StatusCode(500, new { message = "Giriş sırasında bir hata oluştu." });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
