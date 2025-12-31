using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Filters;
using YasamPsikologProject.WebUi.Helpers;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Psychologist/Appointments")]
    //[ServiceFilter(typeof(PsychologistAuthorizationFilter))]
    public class PsychologistAppointmentController : Controller
    {
        private readonly IApiAppointmentService _appointmentService;
        private readonly ILogger<PsychologistAppointmentController> _logger;

        public PsychologistAppointmentController(
            IApiAppointmentService appointmentService,
            ILogger<PsychologistAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Randevularım";

            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    _logger.LogWarning("Psikolog ID bulunamadı");
                    return RedirectToAction("Login", "Account");
                }

                var response = await _appointmentService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    // Sadece kendi randevularını filtrele
                    var appointments = response.Data
                        .Where(a => a.PsychologistId == psychologistId.Value)
                        .OrderByDescending(a => a.AppointmentDate)
                        .ToList();

                    return View(appointments);
                }

                TempData["ErrorMessage"] = response.Message;
                return View(new List<AppointmentDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevular listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<AppointmentDto>());
            }
        }

        [HttpGet]
        [Route("Calendar")]
        public IActionResult Calendar()
        {
            ViewData["PageTitle"] = "Takvim Görünümü";
            return View();
        }

        [HttpGet]
        [Route("GetCalendarEvents")]
        public async Task<IActionResult> GetCalendarEvents()
        {
            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    return Json(new { success = false, message = "Oturum bulunamadı" });
                }

                var response = await _appointmentService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    var appointments = response.Data
                        .Where(a => a.PsychologistId == psychologistId.Value)
                        .Select(a => new
                        {
                            id = a.Id,
                            title = a.ClientName,
                            start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = a.AppointmentEndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            backgroundColor = a.Status == "Pending" ? "#ffc107" :
                                            a.Status == "Confirmed" ? "#28a745" :
                                            a.Status == "Completed" ? "#17a2b8" : "#dc3545",
                            borderColor = a.Status == "Pending" ? "#ffc107" :
                                        a.Status == "Confirmed" ? "#28a745" :
                                        a.Status == "Completed" ? "#17a2b8" : "#dc3545",
                            extendedProps = new
                            {
                                status = a.Status,
                                duration = a.Duration,
                                isOnline = a.IsOnline
                            }
                        })
                        .ToList();

                    return Json(appointments);
                }

                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Takvim verileri yüklenirken hata");
                return Json(new { success = false, message = "Veriler yüklenirken hata oluştu" });
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Randevu Detayları";

            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    _logger.LogWarning("Psikolog ID bulunamadı");
                    return RedirectToAction("Login", "Account");
                }

                var response = await _appointmentService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    // Sadece kendi randevusunu görebilir
                    if (response.Data.PsychologistId != psychologistId.Value)
                    {
                        TempData["ErrorMessage"] = "Bu randevuya erişim yetkiniz yok.";
                        return RedirectToAction(nameof(Index));
                    }

                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("UpdateNotes/{id}")]
        public async Task<IActionResult> UpdateNotes(int id, [FromBody] string notes)
        {
            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    return Json(new { success = false, message = "Oturum bulunamadı" });
                }

                var response = await _appointmentService.GetByIdAsync(id);
                if (!response.Success || response.Data == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı" });
                }

                // Sadece kendi randevusunu güncelleyebilir
                if (response.Data.PsychologistId != psychologistId.Value)
                {
                    return Json(new { success = false, message = "Bu randevuya erişim yetkiniz yok" });
                }

                response.Data.PsychologistNotes = notes;
                var updateResponse = await _appointmentService.UpdateAsync(id, response.Data);

                if (updateResponse.Success)
                {
                    return Json(new { success = true, message = "Notlar başarıyla güncellendi" });
                }

                return Json(new { success = false, message = updateResponse.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notlar güncellenirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu" });
            }
        }

        [HttpPost]
        [Route("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    return Json(new { success = false, message = "Oturum bulunamadı" });
                }

                var response = await _appointmentService.GetByIdAsync(id);
                if (!response.Success || response.Data == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı" });
                }

                // Sadece kendi randevusunu güncelleyebilir
                if (response.Data.PsychologistId != psychologistId.Value)
                {
                    return Json(new { success = false, message = "Bu randevuya erişim yetkiniz yok" });
                }

                response.Data.Status = status;
                var updateResponse = await _appointmentService.UpdateAsync(id, response.Data);

                if (updateResponse.Success)
                {
                    return Json(new { success = true, message = "Randevu durumu güncellendi" });
                }

                return Json(new { success = false, message = updateResponse.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu durumu güncellenirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu" });
            }
        }
    }
}
