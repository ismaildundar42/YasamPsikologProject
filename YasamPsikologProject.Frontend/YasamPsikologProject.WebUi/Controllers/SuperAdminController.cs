using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class SuperAdminController : Controller
    {
        private readonly IApiSuperAdminService _superAdminService;
        private readonly ILogger<SuperAdminController> _logger;

        public SuperAdminController(
            IApiSuperAdminService superAdminService,
            ILogger<SuperAdminController> logger)
        {
            _superAdminService = superAdminService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Süper Admin Yönetimi";
            
            try
            {
                var response = await _superAdminService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }
                
                TempData["ErrorMessage"] = response.Message;
                return View(new List<SuperAdminDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper adminler listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<SuperAdminDto>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["PageTitle"] = "Yeni Süper Admin";
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuperAdminDto model)
        {
            ViewData["PageTitle"] = "Yeni Süper Admin";
            
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("ModelState invalid: {Errors}", errors);
                TempData["ErrorMessage"] = $"Lütfen tüm alanları doğru şekilde doldurunuz. {errors}";
                return View(model);
            }

            try
            {
                // Gerekli alanları kontrol et
                if (string.IsNullOrWhiteSpace(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Ad alanı zorunludur");
                    TempData["ErrorMessage"] = "Ad alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.LastName))
                {
                    ModelState.AddModelError("LastName", "Soyad alanı zorunludur");
                    TempData["ErrorMessage"] = "Soyad alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError("Email", "Email alanı zorunludur");
                    TempData["ErrorMessage"] = "Email alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Telefon alanı zorunludur");
                    TempData["ErrorMessage"] = "Telefon alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Şifre alanı zorunludur");
                    TempData["ErrorMessage"] = "Şifre alanı zorunludur";
                    return View(model);
                }

                _logger.LogInformation("Süper admin oluşturuluyor: {Email}, {FirstName} {LastName}", 
                    model.Email, model.FirstName, model.LastName);
                
                var response = await _superAdminService.CreateAsync(model);
                
                if (response.Success)
                {
                    _logger.LogInformation("Süper admin başarıyla oluşturuldu: {Email}", model.Email);
                    TempData["SuccessMessage"] = "Süper admin başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message ?? "Süper admin oluşturulamadı.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper admin oluşturulurken hata: {Error}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Süper Admin Düzenle";
            
            try
            {
                var response = await _superAdminService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Süper admin bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper admin detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SuperAdminDto model)
        {
            ViewData["PageTitle"] = "Süper Admin Düzenle";

            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return View(model);
            }

            try
            {
                var response = await _superAdminService.UpdateAsync(model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Süper admin başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message ?? "Süper admin güncellenemedi.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper admin güncellenirken hata");
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _superAdminService.DeleteAsync(id);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Süper admin başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Süper admin silinemedi.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper admin silinirken hata");
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Süper Admin Detayları";
            
            try
            {
                var response = await _superAdminService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Süper admin bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süper admin detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
