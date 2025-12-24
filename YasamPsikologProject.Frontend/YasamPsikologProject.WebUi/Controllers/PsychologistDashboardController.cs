using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.ViewModels;
using YasamPsikologProject.WebUi.Filters;
using YasamPsikologProject.WebUi.Helpers;

namespace YasamPsikologProject.WebUi.Controllers
{
    //[Route("Psychologist/Dashboard")]
    //[ServiceFilter(typeof(PsychologistAuthorizationFilter))] // TEMPORARILY DISABLED FOR TESTING
    public class PsychologistDashboardController : Controller
    {
        private readonly IApiAppointmentService _appointmentService;
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiClientService _clientService;
        private readonly ILogger<PsychologistDashboardController> _logger;

        public PsychologistDashboardController(
            IApiAppointmentService appointmentService,
            IApiPsychologistService psychologistService,
            IApiClientService clientService,
            ILogger<PsychologistDashboardController> logger)
        {
            _appointmentService = appointmentService;
            _psychologistService = psychologistService;
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Dashboard";

            var model = new PsychologistDashboardViewModel();

            try
            {
                // TEMPORARY: Using hardcoded ID for testing
                var psychologistId = 8; // HttpContext.Session.GetPsychologistId();
                //if (!psychologistId.HasValue)
                //{
                //    _logger.LogWarning("Psikolog ID bulunamadı");
                //    return RedirectToAction("Login", "Account");
                //}

                // Psikolog bilgilerini çek
                var psychologistResponse = await _psychologistService.GetByIdAsync(psychologistId);
                if (psychologistResponse.Success && psychologistResponse.Data != null)
                {
                    model.PsychologistInfo = psychologistResponse.Data;
                }

                // Tüm randevuları çek (Backend'de filtreleme yapılacak)
                var appointmentsResponse = await _appointmentService.GetAllAsync();
                if (appointmentsResponse.Success && appointmentsResponse.Data != null)
                {
                    // Şimdilik frontend'de filtreleme yapıyoruz (Backend hazır olunca kaldırılacak)
                    var appointments = appointmentsResponse.Data
                        .Where(a => a.PsychologistId == psychologistId)
                        .ToList();

                    var today = DateTime.Today;

                    model.TotalAppointments = appointments.Count;
                    model.TodayAppointments = appointments.Count(a => a.AppointmentDate.Date == today);
                    model.PendingAppointments = appointments.Count(a => a.Status == "Pending");
                    model.CompletedAppointments = appointments.Count(a => a.Status == "Completed");

                    // Son 5 randevu
                    model.RecentAppointments = appointments
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(5)
                        .ToList();

                    // Bugünkü randevular
                    model.TodayAppointmentsList = appointments
                        .Where(a => a.AppointmentDate.Date == today)
                        .OrderBy(a => a.AppointmentDate)
                        .ToList();
                }

                // Atanmış danışan sayısı (Backend'de filtreleme yapılacak)
                var clientsResponse = await _clientService.GetAllAsync();
                if (clientsResponse.Success && clientsResponse.Data != null)
                {
                    // Şimdilik frontend'de filtreleme yapıyoruz
                    model.TotalClients = clientsResponse.Data
                        .Count(c => c.AssignedPsychologistId == psychologistId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu.";
            }

            return View(model);
        }
    }
}
