using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Filters;
using YasamPsikologProject.WebUi.Helpers;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Psychologist/Profile")]
    //[ServiceFilter(typeof(PsychologistAuthorizationFilter))]
    public class PsychologistProfileController : Controller
    {
        private readonly IApiPsychologistService _psychologistService;
        private readonly ILogger<PsychologistProfileController> _logger;

        public PsychologistProfileController(
            IApiPsychologistService psychologistService,
            ILogger<PsychologistProfileController> logger)
        {
            _psychologistService = psychologistService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Profilim";

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();
                //if (!psychologistId.HasValue)
                //{
                //    return RedirectToAction("Login", "Account");
                //}

                // Psikolog bilgilerini çek
                var response = await _psychologistService.GetByIdAsync(psychologistId);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Profil bilgileri yüklenemedi.";
                return RedirectToAction("Index", "PsychologistDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profil bilgileri yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction("Index", "PsychologistDashboard");
            }
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            ViewData["PageTitle"] = "Profil Düzenle";

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();
                //if (!psychologistId.HasValue)
                //{
                //    return RedirectToAction("Login", "Account");
                //}

                var response = await _psychologistService.GetByIdAsync(psychologistId);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Profil bilgileri yüklenemedi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profil düzenlenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PsychologistDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return View(model);
            }

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();
                //if (!psychologistId.HasValue || model.Id != psychologistId.Value)
                //{
                //    TempData["ErrorMessage"] = "Geçersiz işlem.";
                //    return RedirectToAction(nameof(Index));
                //}

                var response = await _psychologistService.UpdateAsync(model.Id, model);

                if (response.Success)
                {
                    // Session'daki kullanıcı adını güncelle
                    if (model.User != null)
                    {
                        HttpContext.Session.SetUserName($"{model.User.FirstName} {model.User.LastName}");
                    }

                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message ?? "Profil güncellenemedi.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profil güncellenirken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                return View(model);
            }
        }

        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword()
        {
            ViewData["PageTitle"] = "Şifre Değiştir";
            return View();
        }

        [HttpPost]
        [Route("ChangePassword")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Tüm alanları doldurunuz.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Yeni şifreler eşleşmiyor.";
                return View();
            }

            if (newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Şifre en az 6 karakter olmalıdır.";
                return View();
            }

            try
            {
                // TODO: Backend'de şifre değiştirme endpoint'i eklenecek
                TempData["InfoMessage"] = "Şifre değiştirme özelliği yakında eklenecektir.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre değiştirilirken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                return View();
            }
        }
    }
}
