using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Helpers;

namespace YasamPsikologProject.WebUi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IApiAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to appropriate dashboard
            if (HttpContext.Session.IsAuthenticated())
            {
                var role = HttpContext.Session.GetString("UserRole");
                return RedirectToDashboard(role);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email ve şifre alanları zorunludur.";
                return View();
            }

            try
            {
                var response = await _authService.LoginAsync(email, password);

                if (response.Success && response.Data != null)
                {
                    var user = response.Data;

                    // Set session
                    HttpContext.Session.SetUserId(user.UserId);
                    HttpContext.Session.SetUserRole(user.Role.ToString());
                    HttpContext.Session.SetUserName($"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetUserEmail(user.Email);

                    if (user.PsychologistId.HasValue)
                    {
                        HttpContext.Session.SetPsychologistId(user.PsychologistId.Value);
                    }

                    if (user.ClientId.HasValue)
                    {
                        HttpContext.Session.SetInt32("ClientId", user.ClientId.Value);
                    }

                    _logger.LogInformation($"User {user.Email} logged in successfully with role {user.Role}");

                    // Redirect based on role
                    return RedirectToDashboard(user.Role.ToString());
                }

                TempData["ErrorMessage"] = response.Message ?? "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                TempData["ErrorMessage"] = "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "0" => RedirectToAction("Index", "Dashboard"), // SuperAdmin
                "1" => RedirectToAction("Index", "PsychologistDashboard"), // Psikolog
                "2" => RedirectToAction("Index", "ClientDashboard"), // Danışan
                _ => RedirectToAction(nameof(Login))
            };
        }
    }
}
