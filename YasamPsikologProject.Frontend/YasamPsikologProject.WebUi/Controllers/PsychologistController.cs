using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class PsychologistController : Controller
    {
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiAppointmentService _appointmentService;
        private readonly ILogger<PsychologistController> _logger;

        public PsychologistController(
            IApiPsychologistService psychologistService,
            IApiAppointmentService appointmentService,
            ILogger<PsychologistController> logger)
        {
            _psychologistService = psychologistService;
            _appointmentService = appointmentService;
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
            ViewData["PageTitle"] = "Yeni Psikolog";
            
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
                    ModelState.AddModelError("", "Kullanıcı bilgileri eksik.");
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri eksik.";
                    return View(model);
                }

                // Gerekli alanları kontrol et
                if (string.IsNullOrWhiteSpace(model.User.FirstName))
                {
                    ModelState.AddModelError("User.FirstName", "Ad alanı zorunludur");
                    TempData["ErrorMessage"] = "Ad alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.LastName))
                {
                    ModelState.AddModelError("User.LastName", "Soyad alanı zorunludur");
                    TempData["ErrorMessage"] = "Soyad alanı zorunludur";
                    return View(model);
                }
 
                if (string.IsNullOrWhiteSpace(model.User.Email))
                {
                    ModelState.AddModelError("User.Email", "Email alanı zorunludur");
                    TempData["ErrorMessage"] = "Email alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.PhoneNumber))
                {
                    ModelState.AddModelError("User.PhoneNumber", "Telefon alanı zorunludur");
                    TempData["ErrorMessage"] = "Telefon alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.Password))
                {
                    ModelState.AddModelError("User.Password", "Şifre alanı zorunludur");
                    TempData["ErrorMessage"] = "Şifre alanı zorunludur";
                    return View(model);
                }

                _logger.LogInformation("Psikolog oluşturuluyor: {Email}, {FirstName} {LastName}, Password: [{Password}]", 
                    model.User.Email, model.User.FirstName, model.User.LastName, model.User.Password);
                
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
            ViewData["PageTitle"] = "Psikolog Düzenle";
            
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
                // User bilgilerini kontrol et
                if (model.User == null)
                {
                    _logger.LogWarning("User bilgisi null");
                    ModelState.AddModelError("", "Kullanıcı bilgileri eksik.");
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri eksik.";
                    return View(model);
                }

                // Gerekli alanları kontrol et
                if (string.IsNullOrWhiteSpace(model.User.FirstName))
                {
                    ModelState.AddModelError("User.FirstName", "Ad alanı zorunludur");
                    TempData["ErrorMessage"] = "Ad alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.LastName))
                {
                    ModelState.AddModelError("User.LastName", "Soyad alanı zorunludur");
                    TempData["ErrorMessage"] = "Soyad alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.Email))
                {
                    ModelState.AddModelError("User.Email", "Email alanı zorunludur");
                    TempData["ErrorMessage"] = "Email alanı zorunludur";
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.User.PhoneNumber))
                {
                    ModelState.AddModelError("User.PhoneNumber", "Telefon alanı zorunludur");
                    TempData["ErrorMessage"] = "Telefon alanı zorunludur";
                    return View(model);
                }

                _logger.LogInformation("Psikolog güncelleniyor: ID={Id}, Email={Email}", id, model.User.Email);
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

        [HttpGet]
        [Route("Archive")]
        public async Task<IActionResult> Archive()
        {
            ViewData["PageTitle"] = "Arşivlenmiş Psikologlar";
            
            try
            {
                var response = await _psychologistService.GetArchivedAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }
                
                TempData["ErrorMessage"] = response.Message;
                return View(new List<PsychologistArchiveDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arşivlenmiş psikologlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<PsychologistArchiveDto>());
            }
        }

        [HttpGet]
        [Route("ArchiveAppointments")]
        public async Task<IActionResult> ArchiveAppointments(int psychologistId)
        {
            ViewData["PageTitle"] = "Arşiv Psikolog Randevuları";
            
            try
            {
                // Arşivlenmiş psikolog bilgisini al
                var archiveResponse = await _psychologistService.GetArchivedAsync();
                var archivedPsychologist = archiveResponse.Data?.FirstOrDefault(p => p.OriginalPsychologistId == psychologistId);
                
                if (archivedPsychologist == null)
                {
                    TempData["ErrorMessage"] = "Arşivlenmiş psikolog bulunamadı.";
                    return RedirectToAction("Archive");
                }

                ViewBag.PsychologistName = $"{archivedPsychologist.FirstName} {archivedPsychologist.LastName}";
                ViewBag.PsychologistId = psychologistId;

                // Bu psikologa ait tüm randevuları getir
                var appointmentsResponse = await _appointmentService.GetByPsychologistAsync(psychologistId);
                
                if (appointmentsResponse.Success && appointmentsResponse.Data != null)
                {
                    return View(appointmentsResponse.Data);
                }
                
                TempData["ErrorMessage"] = appointmentsResponse.Message;
                return View(new List<AppointmentDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arşiv psikolog randevuları listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<AppointmentDto>());
            }
        }
    }
}
