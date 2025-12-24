using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Filters;
using YasamPsikologProject.WebUi.Helpers;
using YasamPsikologProject.WebUi.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Psychologist/WorkingHours")]
    //[ServiceFilter(typeof(PsychologistAuthorizationFilter))]
    public class PsychologistWorkingHourController : Controller
    {
        private readonly IApiWorkingHourService _workingHourService;
        private readonly ILogger<PsychologistWorkingHourController> _logger;

        public PsychologistWorkingHourController(
            IApiWorkingHourService workingHourService,
            ILogger<PsychologistWorkingHourController> logger)
        {
            _workingHourService = workingHourService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Çalışma Saatlerim";

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();

                var response = await _workingHourService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    // Sadece kendi çalışma saatlerini filtrele
                    var workingHours = response.Data
                        .Where(w => w.PsychologistId == psychologistId)
                        .OrderBy(w => w.DayOfWeek)
                        .ThenBy(w => w.StartTime)
                        .ToList();

                    return View(workingHours);
                }

                TempData["ErrorMessage"] = response.Message;
                return View(new List<WorkingHourDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saatleri listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<WorkingHourDto>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["PageTitle"] = "Yeni Çalışma Saati";
            LoadDayOfWeekDropdown();
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkingHourDto model)
        {
            if (!ModelState.IsValid)
            {
                LoadDayOfWeekDropdown();
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return View(model);
            }

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();

                model.PsychologistId = psychologistId;
                var response = await _workingHourService.CreateAsync(model);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Çalışma saati başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message ?? "Çalışma saati eklenemedi.";
                LoadDayOfWeekDropdown();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati eklenirken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                LoadDayOfWeekDropdown();
                return View(model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Çalışma Saati Düzenle";

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();

                var response = await _workingHourService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    // Sadece kendi çalışma saatini düzenleyebilir
                    if (response.Data.PsychologistId != psychologistId)
                    {
                        TempData["ErrorMessage"] = "Bu çalışma saatine erişim yetkiniz yok.";
                        return RedirectToAction(nameof(Index));
                    }

                    LoadDayOfWeekDropdown();
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Çalışma saati bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati düzenlenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkingHourDto model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                LoadDayOfWeekDropdown();
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return View(model);
            }

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();

                // Sadece kendi çalışma saatini güncelleyebilir
                if (model.PsychologistId != psychologistId)
                {
                    TempData["ErrorMessage"] = "Bu çalışma saatine erişim yetkiniz yok.";
                    return RedirectToAction(nameof(Index));
                }

                var response = await _workingHourService.UpdateAsync(id, model);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Çalışma saati başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message ?? "Çalışma saati güncellenemedi.";
                LoadDayOfWeekDropdown();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati güncellenirken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                LoadDayOfWeekDropdown();
                return View(model);
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();

                var response = await _workingHourService.GetByIdAsync(id);
                if (!response.Success || response.Data == null)
                {
                    return Json(new { success = false, message = "Çalışma saati bulunamadı" });
                }

                // Sadece kendi çalışma saatini silebilir
                if (response.Data.PsychologistId != psychologistId)
                {
                    return Json(new { success = false, message = "Bu çalışma saatine erişim yetkiniz yok" });
                }

                var deleteResponse = await _workingHourService.DeleteAsync(id);
                if (deleteResponse.Success)
                {
                    return Json(new { success = true, message = "Çalışma saati başarıyla silindi." });
                }

                return Json(new { success = false, message = deleteResponse.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati silinirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        private void LoadDayOfWeekDropdown()
        {
            ViewBag.DaysOfWeek = new SelectList(new[]
            {
                new { Value = 1, Text = "Pazartesi" },
                new { Value = 2, Text = "Salı" },
                new { Value = 3, Text = "Çarşamba" },
                new { Value = 4, Text = "Perşembe" },
                new { Value = 5, Text = "Cuma" },
                new { Value = 6, Text = "Cumartesi" },
                new { Value = 7, Text = "Pazar" }
            }, "Value", "Text");
        }
    }
}
