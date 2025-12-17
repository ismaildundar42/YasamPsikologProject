using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;
using YasamPsikologProject.WebUi.Models.ViewModels;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class AppointmentController : Controller
    {
        private readonly IApiAppointmentService _appointmentService;
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiClientService _clientService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            IApiAppointmentService appointmentService,
            IApiPsychologistService psychologistService,
            IApiClientService clientService,
            ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _psychologistService = psychologistService;
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Randevu Yönetimi";
            
            try
            {
                var response = await _appointmentService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
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
        public async Task<IActionResult> Calendar()
        {
            ViewData["PageTitle"] = "Randevu Takvimi";
            
            try
            {
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    ViewBag.Psychologists = psychologistsResponse.Data;
                }
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Takvim yüklenirken hata");
                TempData["ErrorMessage"] = "Takvim yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            try
            {
                var response = await _appointmentService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    var events = response.Data.Select(a => new
                    {
                        id = a.Id,
                        title = $"{a.Client?.User?.FirstName} {a.Client?.User?.LastName}",
                        start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = a.AppointmentEndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        backgroundColor = a.Psychologist?.CalendarColor ?? "#3788d8",
                        borderColor = a.Psychologist?.CalendarColor ?? "#3788d8",
                        textColor = "#fff",
                        extendedProps = new
                        {
                            psychologist = $"{a.Psychologist?.User?.FirstName} {a.Psychologist?.User?.LastName}",
                            client = $"{a.Client?.User?.FirstName} {a.Client?.User?.LastName}",
                            status = a.Status,
                            notes = a.Notes
                        }
                    });
                    
                    return Json(events);
                }
                
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Takvim etkinlikleri yüklenirken hata");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["PageTitle"] = "Yeni Randevu";
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                // Tarih kontrolü
                if (model.AppointmentDate < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Geçmiş tarihli randevu oluşturulamaz.";
                    await LoadDropdowns();
                    return View(model);
                }

                var response = await _appointmentService.CreateAsync(model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
                    return RedirectToAction("Index", "Appointment");
                }

                TempData["ErrorMessage"] = response.Message ?? "Randevu oluşturulamadı.";
                await LoadDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu oluşturulurken hata: {Error}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                await LoadDropdowns();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            try
            {
                var response = await _appointmentService.CancelAsync(id, reason);
                if (response.Success)
                {
                    return Json(new { success = true, message = "Randevu başarıyla iptal edildi." });
                }

                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu iptal edilirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int psychologistId, string date, string duration)
        {
            try
            {
                var appointmentDate = DateTime.Parse(date);
                var response = await _appointmentService.GetAvailableSlotsAsync(psychologistId, appointmentDate, duration);
                
                if (response.Success && response.Data != null)
                {
                    var slots = response.Data.Select(s => new
                    {
                        time = s.ToString("HH:mm"),
                        value = s.ToString("yyyy-MM-ddTHH:mm:ss")
                    });
                    
                    return Json(new { success = true, slots });
                }

                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsait saatler yüklenirken hata");
                return Json(new { success = false, message = "Müsait saatler yüklenirken hata oluştu." });
            }
        }

        private async Task LoadDropdowns()
        {
            var psychologistsResponse = await _psychologistService.GetAllAsync();
            if (psychologistsResponse.Success && psychologistsResponse.Data != null)
            {
                ViewBag.Psychologists = psychologistsResponse.Data;
            }
            else
            {
                ViewBag.Psychologists = new List<PsychologistDto>();
            }

            var clientsResponse = await _clientService.GetAllAsync();
            if (clientsResponse.Success && clientsResponse.Data != null)
            {
                ViewBag.Clients = clientsResponse.Data;
            }
            else
            {
                ViewBag.Clients = new List<ClientDto>();
            }
        }
    }
}
