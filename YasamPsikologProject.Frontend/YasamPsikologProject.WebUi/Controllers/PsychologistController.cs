using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class PsychologistController : Controller
    {
        private readonly IApiPsychologistService _psychologistService;
        private readonly ILogger<PsychologistController> _logger;

        public PsychologistController(
            IApiPsychologistService psychologistService,
            ILogger<PsychologistController> logger)
        {
            _psychologistService = psychologistService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Psikolog Yönetimi";
            
            try
            {
                var response = await _psychologistService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }
                
                TempData["ErrorMessage"] = response.Message;
                return View(new List<PsychologistDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikologlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<PsychologistDto>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["PageTitle"] = "Yeni Psikolog";
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PsychologistDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("ModelState invalid: {Errors}", errors);
                TempData["ErrorMessage"] = $"Lütfen tüm alanları doğru şekilde doldurunuz. {errors}";
                return View(model);
            }

            try
            {
                // User bilgilerini kontrol et
                if (model.User == null)
                {
                    _logger.LogWarning("User bilgisi null");
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri eksik.";
                    return View(model);
                }

                _logger.LogInformation("Psikolog oluşturuluyor: {Email}", model.User.Email);
                var response = await _psychologistService.CreateAsync(model);
                
                if (response.Success)
                {
                    _logger.LogInformation("Psikolog başarıyla oluşturuldu: {Email}", model.User.Email);
                    TempData["SuccessMessage"] = "Psikolog başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Psikolog oluşturulamadı: {Message}", response.Message);
                TempData["ErrorMessage"] = response.Message ?? "Psikolog oluşturulamadı.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog oluşturulurken hata: {Message}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Psikolog Düzenle";
            
            try
            {
                var response = await _psychologistService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog düzenlenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PsychologistDto model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("ModelState invalid: {Errors}", errors);
                TempData["ErrorMessage"] = $"Lütfen tüm alanları doğru şekilde doldurunuz. {errors}";
                return View(model);
            }

            try
            {
                _logger.LogInformation("Psikolog güncelleniyor: ID={Id}", id);
                var response = await _psychologistService.UpdateAsync(id, model);
                
                if (response.Success)
                {
                    _logger.LogInformation("Psikolog başarıyla güncellendi: ID={Id}", id);
                    TempData["SuccessMessage"] = "Psikolog başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Psikolog güncellenemedi: {Message}", response.Message);
                TempData["ErrorMessage"] = response.Message ?? "Psikolog güncellenemedi.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog güncellenirken hata: {Error}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Psikolog Detayları";
            
            try
            {
                var response = await _psychologistService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _psychologistService.DeleteAsync(id);
                if (response.Success)
                {
                    return Json(new { success = true, message = "Psikolog başarıyla silindi." });
                }

                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog silinirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }
    }
}
