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
        public async Task<IActionResult> Index(string? status = null)
        {
            ViewData["PageTitle"] = status == "Pending" ? "Bekleyen Randevular" : "Randevularım";

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

                    // Status filtresi varsa uygula
                    if (!string.IsNullOrEmpty(status))
                    {
                        appointments = appointments.Where(a => a.Status == status).ToList();
                    }

                    ViewData["StatusFilter"] = status; // View'da kullanmak için
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
                            title = a.ClientName ?? $"{a.Client?.User?.FirstName} {a.Client?.User?.LastName}",
                            start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = a.AppointmentEndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            backgroundColor = a.Status == "Pending" ? "#ffc107" :
                                            a.Status == "Confirmed" ? "#28a745" :
                                            a.Status == "Completed" ? "#17a2b8" : 
                                            a.Status == "Cancelled" ? "#dc3545" : "#6c757d",
                            borderColor = a.Status == "Pending" ? "#ffc107" :
                                        a.Status == "Confirmed" ? "#28a745" :
                                        a.Status == "Completed" ? "#17a2b8" : 
                                        a.Status == "Cancelled" ? "#dc3545" : "#6c757d",
                            textColor = "#fff",
                            extendedProps = new
                            {
                                status = a.Status,
                                duration = a.Duration,
                                isOnline = a.IsOnline,
                                notes = a.Notes,
                                cancellationReason = a.CancellationReason
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
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? reason = null)
        {
            try
            {
                _logger.LogInformation("UpdateStatus called: id={Id}, status={Status}, reason={Reason}", id, status, reason);
                
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    _logger.LogWarning("UpdateStatus: Session psychologistId not found");
                    return Json(new { success = false, message = "Oturum bulunamadı" });
                }

                var appointmentResponse = await _appointmentService.GetByIdAsync(id);
                if (!appointmentResponse.Success || appointmentResponse.Data == null)
                {
                    _logger.LogWarning("UpdateStatus: Appointment {Id} not found", id);
                    return Json(new { success = false, message = "Randevu bulunamadı" });
                }

                // Sadece kendi randevusunu güncelleyebilir
                if (appointmentResponse.Data.PsychologistId != psychologistId.Value)
                {
                    _logger.LogWarning("UpdateStatus: Unauthorized access attempt by psychologist {PsychId} to appointment {AppId}", 
                        psychologistId.Value, id);
                    return Json(new { success = false, message = "Bu randevuya erişim yetkiniz yok" });
                }

                var response = await _appointmentService.UpdateStatusAsync(id, status, reason);

                if (response.Success)
                {
                    _logger.LogInformation("UpdateStatus: Successfully updated appointment {Id} to {Status}", id, status);
                    return Json(new { success = true, message = "Randevu durumu güncellendi" });
                }

                _logger.LogWarning("UpdateStatus: Failed to update - {Message}", response.Message);
                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateStatus: Exception occurred");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu: " + ex.Message });
            }
        }
    }
}
