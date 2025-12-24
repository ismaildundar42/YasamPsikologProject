using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Filters;
using YasamPsikologProject.WebUi.Helpers;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Psychologist/Clients")]
    //[ServiceFilter(typeof(PsychologistAuthorizationFilter))]
    public class PsychologistClientController : Controller
    {
        private readonly IApiClientService _clientService;
        private readonly IApiAppointmentService _appointmentService;
        private readonly ILogger<PsychologistClientController> _logger;

        public PsychologistClientController(
            IApiClientService clientService,
            IApiAppointmentService appointmentService,
            ILogger<PsychologistClientController> logger)
        {
            _clientService = clientService;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Danışanlarım";

            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var response = await _clientService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    // Sadece kendisine atanmış danışanları filtrele
                    var clients = response.Data
                        .Where(c => c.AssignedPsychologistId == psychologistId.Value)
                        .OrderBy(c => c.User!.FirstName)
                        .ToList();

                    return View(clients);
                }

                TempData["ErrorMessage"] = response.Message;
                return View(new List<ClientDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışanlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<ClientDto>());
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Danışan Detayları";

            try
            {
                var psychologistId = HttpContext.Session.GetPsychologistId();
                if (!psychologistId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                var response = await _clientService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    // Sadece kendi danışanını görebilir
                    if (response.Data.AssignedPsychologistId != psychologistId.Value)
                    {
                        TempData["ErrorMessage"] = "Bu danışana erişim yetkiniz yok.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Danışanın randevu geçmişini çek
                    var appointmentsResponse = await _appointmentService.GetAllAsync();
                    if (appointmentsResponse.Success && appointmentsResponse.Data != null)
                    {
                        ViewBag.ClientAppointments = appointmentsResponse.Data
                            .Where(a => a.ClientId == id && a.PsychologistId == psychologistId.Value)
                            .OrderByDescending(a => a.AppointmentDate)
                            .ToList();
                    }

                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Danışan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
